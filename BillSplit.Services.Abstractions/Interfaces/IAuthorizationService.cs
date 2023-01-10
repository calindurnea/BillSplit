using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IAuthorizationService
{
    Task SetPassword(SetPasswordDto request, CancellationToken cancellationToken = default);
    Task UpdatePassword(SetPasswordDto request, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequest, CancellationToken cancellationToken = default);
}