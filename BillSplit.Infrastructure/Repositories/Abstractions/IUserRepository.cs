using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IUserRepository
{
    Task<User> CreateUser(User user, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken = default);
    Task<User?> GetUsers(string email, CancellationToken cancellationToken = default);
    Task<User?> GetUsers(long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>?> GetUsers(IEnumerable<long> ids, CancellationToken cancellationToken = default);
    Task UpdateUser(User user, CancellationToken cancellationToken = default);
    Task<bool> IsEmailInUse(string email, CancellationToken cancellationToken = default);
    Task<bool> IsPhoneNumberInUse(long phoneNumber, CancellationToken cancellationToken = default);
}