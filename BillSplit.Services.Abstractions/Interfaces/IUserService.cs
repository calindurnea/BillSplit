using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> Get(CancellationToken cancellationToken = default);
    Task<UserDto> Get(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> Get(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task<long> Create(UpsertUserDto request, CancellationToken cancellationToken = default);
    Task Update(long id, UpsertUserDto request, CancellationToken cancellationToken = default);
}