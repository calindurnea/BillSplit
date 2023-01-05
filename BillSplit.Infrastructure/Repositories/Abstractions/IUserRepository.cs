using User = BillSplit.Domain.Models.User;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IUserRepository
{
    Task<User> Create(User userInfo, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> Get(CancellationToken cancellationToken = default);
    Task<User?> Get(string email, CancellationToken cancellationToken = default);
    Task<User?> Get(long id, CancellationToken cancellationToken = default);
    Task Update(User userInfo, CancellationToken cancellationToken = default);
}
