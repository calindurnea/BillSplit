using BillSplit.Domain.Entities;

namespace BillSplit.Persistance.Repositories.Abstractions;

public interface IUserRepository
{
    Task<UserEntity> Create(UserEntity userInfo, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserEntity>> Get(CancellationToken cancellationToken = default);
    Task<UserEntity?> Get(string email, CancellationToken cancellationToken = default);
    Task<UserEntity?> Get(long id, CancellationToken cancellationToken = default);
    Task Update(UserEntity userInfo, CancellationToken cancellationToken = default);
}
