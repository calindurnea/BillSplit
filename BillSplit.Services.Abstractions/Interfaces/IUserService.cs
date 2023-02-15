using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken = default);
    Task<UserDto> GetUser(long id);
    Task<IEnumerable<UserDto>> GetUsers(ISet<long> ids, CancellationToken cancellationToken = default);
    Task<long> CreateUser(UpsertUserDto request);
    Task UpdateUser(long id, UpsertUserDto request);
}