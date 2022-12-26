using BillSplit.Domain.Entities;
using BillSplit.Persistance.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistance.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public UserRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<long> Create(UserEntity user, CancellationToken cancellationToken = default)
    {
        var result = await _applicationContext.Users.AddAsync(user, cancellationToken);
        await _applicationContext.SaveChangesAsync(cancellationToken);

        return result.Entity.Id;
    }

    public async Task<IEnumerable<UserEntity>> Get(CancellationToken cancellationToken = default)
    {
        return await _applicationContext.Users.ToListAsync(cancellationToken);
    }

    public async Task Update(UserEntity user, CancellationToken cancellationToken = default)
    {
        _applicationContext.Users.Update(user);
        await _applicationContext.SaveChangesAsync(cancellationToken);
    }
}