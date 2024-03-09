using System.Globalization;
using System.Net;
using System.Security.Claims;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using BillSplit.Domain.Models;
using BillSplit.Domain.ResultHandling;
using BillSplit.Persistence.Caching;
using BillSplit.Services.Abstractions.Interfaces;
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

    public async Task<IResult<bool>> SetInitialPassword(SetInitialPasswordDto request)
    {
        if (!string.Equals(request.Password, request.PasswordCheck, StringComparison.Ordinal))
        {
            return Result.Failure<bool>("The new passwords does not match the confirm password", HttpStatusCode.BadRequest);
        }

        var user = await _userManager.FindByIdAsync(request.UserId.ToString(CultureInfo.InvariantCulture));

        if (user is null)
        {
            return Result.Failure<bool>("The specified user does not exist", HttpStatusCode.NotFound);
        }

        var identityResult = await _userManager.AddPasswordAsync(user, request.Password);

        if (!identityResult.Succeeded)
        {
            return Result.Failure<bool>("Password was not successfully set",
                HttpStatusCode.BadRequest,
                identityResult.Errors
                    .Select(x => x.Description)
                    .ToArray());
        }

        return Result.Success(true);
    }

    public async Task<IResult<bool>> UpdatePassword(ClaimsPrincipal principal, UpdatePasswordDto request)
    {
        if (!string.Equals(request.NewPassword, request.NewPasswordCheck, StringComparison.Ordinal))
        {
            return Result.Failure<bool>("The new passwords does not match the confirm password", HttpStatusCode.BadRequest);
        }

        var user = await _userManager.GetUserAsync(principal);

        if (user is null)
        {
            return Result.Failure<bool>("The specified user does not exist", HttpStatusCode.NotFound);
        }

        var identityResult = await _userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);

        if (!identityResult.Succeeded)
        {
            return Result.Failure<bool>("Password was not successfully changed",
                HttpStatusCode.BadRequest,
                identityResult.Errors
                    .Select(x => x.Description)
                    .ToArray());
        }

        await Logout(user.Id);
        return Result.Success(true);
    }

    public async Task<IResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Failure<LoginResponseDto>("Wrong username or password", HttpStatusCode.Unauthorized);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!isPasswordValid)
        {
            return Result.Failure<LoginResponseDto>("Wrong username or password", HttpStatusCode.Unauthorized);
        }

        await Logout(user.Id);

        var loginTokensResult = await GenerateLoginTokens(user);

        if (loginTokensResult is not Result.ISuccessResult<(AccessTokenResult, RefreshTokenResult)> loginTokens)
        {
            return Result.Failure<LoginResponseDto, (AccessTokenResult, RefreshTokenResult)>(loginTokensResult);
        }

        var (accessTokenResult, refreshTokenResult) = loginTokens.Result;

        return Result.Success(new LoginResponseDto(accessTokenResult.Token, refreshTokenResult.Token, accessTokenResult.ExpiresOn));
    }

    private async Task<IResult<(AccessTokenResult, RefreshTokenResult)>> GenerateLoginTokens(User user)
    {
        var tokenResult = _jwtTokenGenerator.CreateToken(user);

        if (tokenResult is not Result.ISuccessResult<AccessTokenResult> token)
        {
            return Result.Failure<(AccessTokenResult, RefreshTokenResult), AccessTokenResult>(tokenResult);
        }

        var refreshTokenResult = _jwtTokenGenerator.CreateRefreshToken(user);

        if (refreshTokenResult is not Result.ISuccessResult<RefreshTokenResult> refreshToken)
        {
            return Result.Failure<(AccessTokenResult, RefreshTokenResult), RefreshTokenResult>(refreshTokenResult);
        }

        await _cacheManger.SetData(
            LoggedUserCacheKeyPrefix + user.Id,
            user.Id.ToString(CultureInfo.InvariantCulture),
            token.Result.ExpiresOn - DateTime.UtcNow + TimeSpan.FromSeconds(30));

        await _cacheManger.PrePendData(RefreshTokenCacheKeyPrefix + user.Id, refreshToken.Result.Token);
        return Result.Success((token.Result, refreshToken.Result));
    }

    public async Task<IResult<bool>> Logout(long userId)
    {
        var result = await _cacheManger.RemoveData(LoggedUserCacheKeyPrefix + userId);

        return result switch
        {
            true => Result.Success(result),
            false => Result.Failure<bool>(new ResultError("Logout was not successful", HttpStatusCode.InternalServerError))
        };
    }

    public async Task<IResult<LoginResponseDto>> RefreshToken(TokenRefreshRequestDto request)
    {
        var userClaimsResult = GetValidatedUserClaims(request);

        if (userClaimsResult is not Result.ISuccessResult<UserClaims> userClaims)
        {
            return Result.Failure<LoginResponseDto, UserClaims>(userClaimsResult);
        }

        var user = await _userManager.FindByEmailAsync(userClaims.Result.Email);
        
        if (user is null)
        {
            InvalidRefreshTokenLogger(_logger, $"User not found. User email: `{userClaims.Result.Email}`", null);
            return Result.Failure<LoginResponseDto>("Invalid request", HttpStatusCode.Unauthorized);
        }

        var validationResult = await ValidateRefreshToken(request, user);

        if (validationResult is Result.IFailureResult<bool>)
        {
            return Result.Failure<LoginResponseDto, bool>(validationResult);
        }

        // by this point:
        // request data is valid (user claims, access token and refresh token ids and emails match
        // the user exists in the db
        // the refresh token was found and it is the last generated one

        var loginTokensResult = await GenerateLoginTokens(user);

        if (loginTokensResult is not Result.ISuccessResult<(AccessTokenResult, RefreshTokenResult)> loginTokens)
        {
            return Result.Failure<LoginResponseDto, (AccessTokenResult, RefreshTokenResult)>(loginTokensResult);
        }

        var (accessTokenResult, refreshTokenResult) = loginTokens.Result;
        return Result.Success(new LoginResponseDto(accessTokenResult.Token, refreshTokenResult.Token, accessTokenResult.ExpiresOn));
    }

    private async Task<IResult<bool>> ValidateRefreshToken(TokenRefreshRequestDto request, User user)
    {
        var refreshTokens = await _cacheManger.GetData<string>(RefreshTokenCacheKeyPrefix + user.Id, 0, 9);

        if (refreshTokens.Length < 1)
        {
            InvalidRefreshTokenLogger(_logger, $"No refresh token was found for the user: `{user.Id}`", null);
            return Result.Failure<bool>("Invalid request", HttpStatusCode.Unauthorized);
        }

        // validate if the list has the token but not as the most recent (refresh token is reused => bad)
        if (!string.Equals(refreshTokens[0], request.RefreshToken, StringComparison.Ordinal) &&
            refreshTokens.Contains(request.RefreshToken, StringComparer.Ordinal))
        {
            InvalidRefreshTokenLogger(_logger, $"Refresh token has been already used: `{request.RefreshToken}`", null);

            // This is the damage control in case of tokens being compromised and replay attacks
            // await _cacheManger.RemoveData(LoggedUserCacheKeyPrefix + user.Id);
            // await _cacheManger.RemoveData(RefreshTokenCacheKeyPrefix + user.Id);
            // await _userManager.RemovePasswordAsync(user);

            return Result.Failure<bool>("Invalid request", HttpStatusCode.Unauthorized);
        }

        // latest refresh token is not the one used
        if (!string.Equals(refreshTokens[0], request.RefreshToken, StringComparison.Ordinal))
        {
            InvalidRefreshTokenLogger(_logger, $"Refresh token does not match: `{request.RefreshToken}`", null);
            return Result.Failure<bool>("Invalid request", HttpStatusCode.Unauthorized);
        }

        return Result.Success(true);
    }

    private IResult<UserClaims> GetValidatedUserClaims(TokenRefreshRequestDto request)
    {
        // validate access token is correct and has claims
        var accessTokenClaimsResult = _jwtTokenGenerator.TryGetClaimsFromExpiredToken(request.Token); 
        if (accessTokenClaimsResult is not Result.ISuccessResult<HashSet<Claim>> accessTokenClaims)
        {
            InvalidRefreshTokenLogger(_logger, $"Invalid access token: `{request.Token}`", null);
            return Result.Failure<UserClaims>("Invalid request", HttpStatusCode.Unauthorized);
        }

        // validate refresh token and get data
        var deconstructedRefreshTokenResult = _jwtTokenGenerator.DeconstructRefreshToken(request.RefreshToken);
        if (deconstructedRefreshTokenResult is not Result.ISuccessResult<DeconstructedRefreshToken> deconstructedRefreshToken)
        {
            InvalidRefreshTokenLogger(_logger, $"Invalid refresh token: `{request.RefreshToken}`", null);
            return Result.Failure<UserClaims>("Invalid request", HttpStatusCode.Unauthorized);
        }

        if (deconstructedRefreshToken.Result.Expiry < DateTime.UtcNow)
        {
            InvalidRefreshTokenLogger(_logger, $"Expired refresh token: `{request.RefreshToken}`", null);
            return Result.Failure<UserClaims>("Invalid request", HttpStatusCode.Unauthorized);
        }

        var accessTokenId = long.Parse(accessTokenClaims.Result.First(x => x.Type == ClaimTypes.NameIdentifier).Value, CultureInfo.InvariantCulture);
        var accessTokenEmail = accessTokenClaims.Result.First(x => x.Type == ClaimTypes.Email).Value;

        // validate ids match
        if (accessTokenId != deconstructedRefreshToken.Result.Id)
        {
            InvalidRefreshTokenLogger(_logger, $"Mismatching ids. Found: `{accessTokenId}, {deconstructedRefreshToken.Result.Id}`", null);
            return Result.Failure<UserClaims>("Invalid request", HttpStatusCode.Unauthorized);
        }

        // validate emails match
        if (!string.Equals(accessTokenEmail, deconstructedRefreshToken.Result.Email, StringComparison.OrdinalIgnoreCase))
        {
            InvalidRefreshTokenLogger(_logger, $"Mismatching emails. Found: `{accessTokenEmail}, {deconstructedRefreshToken.Result.Email}`", null);
            return Result.Failure<UserClaims>("Invalid request", HttpStatusCode.Unauthorized);
        }

        return Result.Success(new UserClaims(accessTokenId, accessTokenEmail));
    }
}