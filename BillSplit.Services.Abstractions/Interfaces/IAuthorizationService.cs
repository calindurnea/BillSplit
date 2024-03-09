using System.Security.Claims;
using BillSplit.Contracts.Authorization;
using BillSplit.Domain.ResultHandling;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IAuthorizationService
{
    Task<IResult<bool>> SetInitialPassword(SetInitialPasswordDto request);
    Task<IResult<bool>> UpdatePassword(ClaimsPrincipal user, UpdatePasswordDto request);
    Task<IResult<LoginResponseDto>> Login(LoginRequestDto request);
    Task<IResult<bool>> Logout(long userId);
    Task<IResult<LoginResponseDto>> RefreshToken(TokenRefreshRequestDto request);
}