using BillSplit.Domain.Entities;

namespace BillSplit.Persistance.Repositories.Abstractions;

public interface IUserRepository
{
    Task<long> Create(UserEntity userInfo, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserEntity>> Get(CancellationToken cancellationToken = default);
    Task Update(UserEntity userInfo, CancellationToken cancellationToken = default);
}
