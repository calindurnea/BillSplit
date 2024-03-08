using BillSplit.Domain.Models;
using BillSplit.Persistence.Extensions;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories;

internal sealed class BillGroupRepository : IBillGroupRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public BillGroupRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<IEnumerable<BillGroup>> GetBillGroups(CancellationToken cancellationToken, bool withNoTracking = true, params long[] ids)
    {
        return await _applicationContext.BillGroups
            .WithNoTracking(withNoTracking)
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<BillGroup> CreateBillGroup(BillGroup billGroup, CancellationToken cancellationToken)
    {
        var result = await _applicationContext.BillGroups.AddAsync(billGroup, cancellationToken);
        await _applicationContext.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task UpdateBillGroup(BillGroup billGroup, CancellationToken cancellationToken)
    {
        _applicationContext.BillGroups.Update(billGroup);
        await _applicationContext.SaveChangesAsync(cancellationToken);
    }
}