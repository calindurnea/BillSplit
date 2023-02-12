using BillSplit.Domain.Models;
using BillSplit.Persistence.Extensions;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public UserRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<User> CreateUser(User user, CancellationToken cancellationToken = default)
    {
        var result = await _applicationContext.Users.AddAsync(user, cancellationToken);
        await _applicationContext.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<IEnumerable<User>> GetUsers(CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users.WithNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task<User?> GetUsers(string email, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users
            .WithNoTracking()
            .FirstOrDefaultAsync(x => string.Equals(x.Email, email) && !x.IsDeleted, cancellationToken);
    }

    public async Task<User?> GetUsers(long id, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<User>?> GetUsers(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users
            .WithNoTracking()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateUser(User user, CancellationToken cancellationToken = default)
    {
        _applicationContext.Users.Update(user);
        await _applicationContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> IsEmailInUse(string email, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users.WithNoTracking()
                .FirstOrDefaultAsync(x => x.Email == email && !x.IsDeleted, cancellationToken: cancellationToken)
            is not null;
    }

    public async Task<bool> IsPhoneNumberInUse(long phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users.WithNoTracking()
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && !x.IsDeleted, cancellationToken: cancellationToken)
            is not null;
    }
}