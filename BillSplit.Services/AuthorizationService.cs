using System.Security.Authentication;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Identity;

namespace BillSplit.Services;

internal sealed class AuthorizationService : IAuthorizationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthorizationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task SetInitialPassword(SetInitialPasswordDto request, CancellationToken cancellationToken = default)
    {
        if (string.Equals(request.Password, request.PasswordCheck, StringComparison.Ordinal))
        {
            throw new PasswordCheckException("The password did not match with the repeated password");
        }

        var user = (await _userRepository.GetUsers(request.UserId, cancellationToken)).ThrowIfNull(request.UserId);

        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            throw new PasswordCheckException("User has a password already set");
        }

        await UpdatePassword(user.Id, request.Password, cancellationToken);
    }

    public async Task UpdatePassword(UserClaims user, UpdatePasswordDto request, CancellationToken cancellationToken = default)
    {
        if (!string.Equals(request.NewPassword, request.NewPasswordCheck, StringComparison.Ordinal))
        {
            throw new PasswordCheckException("The new password did not match with the repeated new password");
        }

        var existingUser = (await _userRepository.GetUsers(user.Id, cancellationToken)).ThrowIfNull(user.Id);

        var passwordCheck = VerifyPassword(existingUser, existingUser.Password!, request.Password);

        if (passwordCheck == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Wrong username or password");
        }

        await UpdatePassword(existingUser.Id, request.NewPassword, cancellationToken);
    }

    private async Task UpdatePassword(long userId, string password, CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.GetUsers(userId, cancellationToken)).ThrowIfNull(userId);

        var passwordHasher = new PasswordHasher<User>();
        var hash = passwordHasher.HashPassword(user, password);

        // should I? or can it be maliciously used to find passwords?
        if (string.Equals(user.Password, hash, StringComparison.Ordinal))
        {
            throw new PasswordCheckException("The new password must differ from the current password");
        }

        user.Password = hash;
        await _userRepository.UpdateUser(user, cancellationToken);
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetUsers(loginRequest.Email, cancellationToken);

        if (user is null)
        {
            throw new AuthenticationException("Wrong username or password");
        }

        if (user.Password is null)
        {
            throw new PasswordCheckException("No password was set for the current user");
        }

        var passwordCheck = VerifyPassword(user, user.Password, loginRequest.Password);

        if (passwordCheck == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Wrong username or password");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id);

        return new LoginResponseDto(token);
    }

    private static PasswordVerificationResult VerifyPassword(User user, string passwordHash, string providedPassword)
    {
        var passwordHasher = new PasswordHasher<User>();
        return passwordHasher.VerifyHashedPassword(user, passwordHash, providedPassword);
    }
}