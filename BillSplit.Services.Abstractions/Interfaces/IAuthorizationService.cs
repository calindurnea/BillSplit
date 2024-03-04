using System.Security.Claims;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IAuthorizationService
{
    Task SetInitialPassword(SetInitialPasswordDto request);
    Task UpdatePassword(ClaimsPrincipal user, UpdatePasswordDto request);
    Task<LoginResponseDto> Login(LoginRequestDto request);
    Task Logout(long userId);
    Task<LoginResponseDto> RefreshToken(TokenRefreshRequestDto request);
}