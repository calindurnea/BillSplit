using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IAuthorizationService
{
    Task SetInitialPassword(SetInitialPasswordDto request, CancellationToken cancellationToken = default);
    Task UpdatePassword(UserClaims user, UpdatePasswordDto request, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequest, CancellationToken cancellationToken = default);
}