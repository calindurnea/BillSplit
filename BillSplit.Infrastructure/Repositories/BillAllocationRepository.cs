using BillSplit.Domain.Models;
using BillSplit.Persistence.Extensions;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories;

internal class BillAllocationRepository : IBillAllocationRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public BillAllocationRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<IEnumerable<BillAllocation>> GetBillAllocations(long billId, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.BillAllocations
            .WithNoTracking()
            .Where(x => x.BillId == billId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BillAllocation>> GetBillsAllocations(IEnumerable<long> billIds, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.BillAllocations
            .WithNoTracking()
            .Where(x => billIds.Contains(x.BillId) && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BillAllocation>> GetUserBillGroupAllocations(long userId, long billGroupId, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.BillAllocations
            .WithNoTracking()
            .Where(x => x.UserId == userId && x.Bill.BillGroupId == billGroupId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BillAllocation>> GetBillGroupAllocations(long billGroupId, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.BillAllocations
            .WithNoTracking()
            .Where(x => x.Bill.BillGroupId == billGroupId && !x.IsDeleted)
            .ToListAsync(cancellationToken);
    }
}