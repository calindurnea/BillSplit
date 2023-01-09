using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> Get(CancellationToken cancellationToken = default);
    Task<UserDto> Get(long id, CancellationToken cancellationToken = default);
    Task<long> Create(CreateUserDto request, CancellationToken cancellationToken = default);
    Task SetPassword(SetPasswordDto request, CancellationToken cancellationToken = default);
    Task UpdatePassword(SetPasswordDto request, CancellationToken cancellationToken = default);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequest, CancellationToken cancellationToken = default);
}