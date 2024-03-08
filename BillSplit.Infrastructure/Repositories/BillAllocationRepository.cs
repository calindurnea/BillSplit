using BillSplit.Domain.Models;
using BillSplit.Persistence.Extensions;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories;

internal sealed class BillAllocationRepository : IBillAllocationRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public BillAllocationRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<IEnumerable<BillAllocation>> GetUserBillGroupAllocations(long userId, long billGroupId, CancellationToken cancellationToken)
    {
        return await _applicationContext.BillAllocations
            .WithNoTracking()
            .Where(x => x.UserId == userId && x.Bill.BillGroupId == billGroupId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BillAllocation>> GetBillGroupAllocations(long billGroupId, CancellationToken cancellationToken)
    {
        return await _applicationContext.BillAllocations
            .WithNoTracking()
            .Where(x => x.Bill.BillGroupId == billGroupId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}