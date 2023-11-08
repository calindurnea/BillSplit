using System.Globalization;
using System.Security.Authentication;
using System.Security.Claims;
using BillSplit.Contracts.Authorization;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Caching;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Identity;

namespace BillSplit.Services;

internal sealed class AuthorizationService : IAuthorizationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly UserManager<User> _userManager;
    private readonly ICacheManger _cacheManger;

    public AuthorizationService(
        IJwtTokenGenerator jwtTokenGenerator,
        UserManager<User> userManager,
        ICacheManger cacheManger)
    {
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _cacheManger = cacheManger ?? throw new ArgumentNullException(nameof(cacheManger));
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

        var tokenResult = _jwtTokenGenerator.CreateToken(user);

        _cacheManger.SetData(
            "authorization:logged:users:" + user.Id,
            user.Id.ToString(CultureInfo.InvariantCulture),
            TimeSpan.FromMinutes(5));

        return new LoginResponseDto(tokenResult.Token, tokenResult.ExpiresOn);
    }

    public Task Logout(long userId)
    {
        return Task.FromResult(_cacheManger.RemoveData("authorization:logged:users:" + userId));
    }
}