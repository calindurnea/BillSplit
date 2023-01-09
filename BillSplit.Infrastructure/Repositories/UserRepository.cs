using BillSplit.Domain.Models;
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

    public async Task<User> Create(User user, CancellationToken cancellationToken = default)
    {
        var result = await _applicationContext.Users.AddAsync(user, cancellationToken);
        await _applicationContext.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<IEnumerable<User>> Get(CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<User?> Get(string email, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => string.Equals(x.Email, email), cancellationToken);
    }

    public async Task<User?> Get(long id, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task Update(User user, CancellationToken cancellationToken = default)
    {
        _applicationContext.Users.Update(user);
        await _applicationContext.SaveChangesAsync(cancellationToken);
    }
}