using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IUserRepository
{
    Task<User> Create(User user, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> Get(CancellationToken cancellationToken = default);
    Task<User?> Get(string email, CancellationToken cancellationToken = default);
    Task<User?> Get(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>?> Get(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task Update(User user, CancellationToken cancellationToken = default);
    Task<bool> IsEmailInUse(string email, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneNumberInUse(long phoneNumber, CancellationToken cancellationToken = default);
}