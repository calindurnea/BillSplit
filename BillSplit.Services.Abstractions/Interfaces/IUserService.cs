using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken = default);
    Task<UserDto> GetUsers(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> GetUsers(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<long> CreateUser(UpsertUserDto request, CancellationToken cancellationToken = default);
    Task UpdateUser(long id, UpsertUserDto request, CancellationToken cancellationToken = default);
}