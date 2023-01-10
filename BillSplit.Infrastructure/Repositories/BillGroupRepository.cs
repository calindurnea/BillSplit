using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories;

internal class BillGroupRepository : IBillGroupRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public BillGroupRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<IEnumerable<BillGroup>?> GetByUserId(long userId, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.BillGroups
            .AsNoTracking()
            .Where(billGroup => billGroup.CreatedBy == userId && billGroup.IsDeleted == false)
            .Include(billGroup => billGroup.Bills)
            .ToListAsync(cancellationToken);
    }

    public async Task<BillGroup?> Get(long id, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.BillGroups
            .AsNoTracking()
            .Include(billGroup => billGroup.Bills)
            .Include(billGroup => billGroup.UserBillGroups)
            .FirstOrDefaultAsync(billGroup => billGroup.Id == id, cancellationToken);
    }

    public async Task<BillGroup> Create(BillGroup billGroup, CancellationToken cancellationToken = default)
    {
        var result = await _applicationContext.BillGroups.AddAsync(billGroup, cancellationToken);
        await _applicationContext.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }
}