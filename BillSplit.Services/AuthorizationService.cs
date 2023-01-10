using System.Security.Authentication;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Identity;

namespace BillSplit.Services;

internal class AuthorizationService : IAuthorizationService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthorizationService(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task SetPassword(SetPasswordDto request, CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.Get(request.UserId, cancellationToken)).ThrowIfNull(request.UserId);

        if (!string.IsNullOrWhiteSpace(user.Password))
        {
            throw new PasswordCheckException("User has a password already set");
        }

        await UpdatePassword(request, cancellationToken);
    }

    public async Task UpdatePassword(SetPasswordDto request, CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.Get(request.UserId, cancellationToken)).ThrowIfNull(request.UserId);

        var passwordHasher = new PasswordHasher<User>();
        var hash = passwordHasher.HashPassword(user, request.Password);

        var passwordCheck = passwordHasher.VerifyHashedPassword(user, hash, request.PasswordCheck);

        if (passwordCheck == PasswordVerificationResult.Failed)
        {
            throw new PasswordCheckException("The password did not match with the repeated password");
        }

        // should I? or can it be maliciously used to find passwords?
        if (string.Equals(user.Password, hash))
        {
            throw new PasswordCheckException("The new password must differ from the current password");
        }

        user.Password = hash;
        await _userRepository.Update(user, cancellationToken);
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.Get(loginRequest.Email, cancellationToken);

        if (user is null)
        {
            throw new AuthenticationException("Wrong username or password");
        }

        if (user.Password is null)
        {
            throw new PasswordCheckException("No password was set for the current user");
        }

        var passwordHasher = new PasswordHasher<User>();
        var passwordCheck = passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);

        if (passwordCheck == PasswordVerificationResult.Failed)
        {
            throw new AuthenticationException("Wrong username or password");
        }

        var token = _jwtTokenGenerator.Generate(user.Id);

        return new LoginResponseDto(token);
    }
}