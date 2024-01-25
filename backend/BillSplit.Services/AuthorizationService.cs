using System.Globalization;
using System.Security.Authentication;
using System.Security.Claims;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Caching;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace BillSplit.Services;

internal sealed class AuthorizationService : IAuthorizationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<User> _userManager;
    private readonly ICacheManger _cacheManger;
    private readonly ILogger<AuthorizationService> _logger;

    private const string RefreshTokenCacheKeyPrefix = "authorization:refreshtokens:users:";
    private const string LoggedUserCacheKeyPrefix = "authorization:logged:users:";

    private static readonly Action<ILogger, string, Exception?> InvalidRefreshTokenLogger =
        LoggerMessage.Define<string>(
            LogLevel.Critical,
            new EventId(3, "InvalidRefreshTokenError"),
            formatString: "Error when validating refresh token. Reason: `{Reason}`");

    public AuthorizationService(
        IJwtTokenGenerator jwtTokenGenerator,
        UserManager<User> userManager,
        ICacheManger cacheManger,
        ILogger<AuthorizationService> logger)
    {
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _cacheManger = cacheManger ?? throw new ArgumentNullException(nameof(cacheManger));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task SetInitialPassword(SetInitialPasswordDto request)
    {
        if (!string.Equals(request.Password, request.PasswordCheck, StringComparison.Ordinal))
        {
            throw new PasswordCheckException("The password did not match with the repeated password");
        }

        var user = (await _userManager.FindByIdAsync(request.UserId.ToString(CultureInfo.InvariantCulture))).ThrowIfNull();
        await _userManager.AddPasswordAsync(user, request.Password);
    }

    public async Task UpdatePassword(ClaimsPrincipal principal, UpdatePasswordDto request)
    {
        if (!string.Equals(request.NewPassword, request.NewPasswordCheck, StringComparison.Ordinal))
        {
            throw new PasswordCheckException("The new password did not match with the repeated new password");
        }

        var user = (await _userManager.GetUserAsync(principal)).ThrowIfNull();

        await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        await Logout(user.Id);
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new AuthenticationException("Wrong username or password");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            throw new AuthenticationException("Wrong username or password");
        }

        var (accessTokenResult, refreshTokenResult) = await GenerateLoginTokens(user);
        return new LoginResponseDto(accessTokenResult.Token, refreshTokenResult.Token, accessTokenResult.ExpiresOn);
    }

    private async Task<(AccessTokenResult, RefreshTokenResult)> GenerateLoginTokens(User user)
    {
        var tokenResult = _jwtTokenGenerator.CreateToken(user);
        var refreshTokenResult = _jwtTokenGenerator.CreateRefreshToken(user);

        await _cacheManger.SetData(
            LoggedUserCacheKeyPrefix + user.Id,
            user.Id.ToString(CultureInfo.InvariantCulture),
            tokenResult.ExpiresOn - DateTime.UtcNow + TimeSpan.FromSeconds(30));

        await _cacheManger.PrePendData(RefreshTokenCacheKeyPrefix + user.Id, refreshTokenResult);
        return (tokenResult, refreshTokenResult);
    }

    public async Task Logout(long userId)
    {
        await _cacheManger.RemoveData(LoggedUserCacheKeyPrefix + userId);
    }

    public async Task<LoginResponseDto> RefreshToken(UserClaims userClaims, TokenRefreshRequestDto request)
    {
        ValidateReceivedData(userClaims, request);

        var user = await _userManager.FindByEmailAsync(userClaims.Email);

        if (user is null)
        {
            InvalidRefreshTokenLogger(_logger, $"User not found. User email: `{userClaims.Email}`", null);
            throw new AuthenticationException("Invalid request");
        }

        await ValidateRefreshToken(request, user);

        // by this point:
            // request data is valid (user claims, access token and refresh token ids and emails match
            // the user exists in the db
            // the refresh token was found and it is the last generated one

        var (accessTokenResult, refreshTokenResult) = await GenerateLoginTokens(user);
        return new LoginResponseDto(accessTokenResult.Token, refreshTokenResult.Token, accessTokenResult.ExpiresOn);
    }

    private async Task ValidateRefreshToken(TokenRefreshRequestDto request, User user)
    {
        var refreshTokens = await _cacheManger.GetData<string>(RefreshTokenCacheKeyPrefix + user.Id, 0, 9);

        // validate if the list has the token but not as the most recent (refresh token is reused => bad)
        if (!string.Equals(refreshTokens[0], request.RefreshToken, StringComparison.Ordinal) &&
            refreshTokens.Contains(request.RefreshToken, StringComparer.Ordinal))
        {
            InvalidRefreshTokenLogger(_logger, $"Refresh token has been already used: `{request.RefreshToken}`", null);

            // This is the damage control in case of tokens being compromised and replay attacks
            // await _cacheManger.RemoveData(LoggedUserCacheKeyPrefix + user.Id);
            // await _cacheManger.RemoveData(RefreshTokenCacheKeyPrefix + user.Id);
            // await _userManager.RemovePasswordAsync(user);

            throw new AuthenticationException("Invalid request");
        }

        // latest refresh token is not the one used
        if (!string.Equals(refreshTokens[0], request.RefreshToken, StringComparison.Ordinal))
        {
            InvalidRefreshTokenLogger(_logger, $"Refresh token does not match: `{request.RefreshToken}`", null);
            throw new AuthenticationException("Invalid request");
        }
    }

    private void ValidateReceivedData(UserClaims user, TokenRefreshRequestDto request)
    {
        // validate access token is correct and has claims
        if (!_jwtTokenGenerator.TryGetClaimsFromExpiredToken(request.Token, out var accessTokenClaims))
        {
            InvalidRefreshTokenLogger(_logger, $"Invalid access token: `{request.Token}`", null);
            throw new AuthenticationException("Invalid request");
        }

        // validate refresh token and get data
        if (!_jwtTokenGenerator.TryGetUserClaimsFromRefreshToken(request.RefreshToken, out var refreshTokenClaims))
        {
            InvalidRefreshTokenLogger(_logger, $"Invalid refresh token: `{request.RefreshToken}`", null);
            throw new AuthenticationException("Invalid request");
        }

        var accessTokenId = long.Parse(accessTokenClaims.First(x => x.Type == ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
        var accessTokenEmail = accessTokenClaims.First(x => x.Type == ClaimTypes.Email).Value;

        var ids = new List<long> { user.Id, accessTokenId, refreshTokenClaims.Id };
        var emails = new List<string> { user.Email, accessTokenEmail, refreshTokenClaims.Email };

        // validate all found ids match
        if (ids.Distinct().Count() != 1)
        {
            InvalidRefreshTokenLogger(_logger, $"Mismatching ids. Found: `{string.Join(',', ids)}`", null);
            throw new AuthenticationException("Invalid request");
        }

        // validate all found emails match
        if (emails.Distinct().Count() != 1)
        {
            InvalidRefreshTokenLogger(_logger, $"Mismatching emails. Found: `{string.Join(',', emails)}`", null);
            throw new AuthenticationException("Invalid request");
        }
    }
}