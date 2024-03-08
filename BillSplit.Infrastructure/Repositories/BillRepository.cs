using BillSplit.Domain.Models;
using BillSplit.Persistence.Extensions;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories;

internal sealed class BillRepository : IBillRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public BillRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<Bill> CreateBill(Bill bill, CancellationToken cancellationToken)
    {
        var result = await _applicationContext.Bills.AddAsync(bill, cancellationToken);
        await _applicationContext.SaveChangesAsync(cancellationToken);

        return result.Entity;
    }

    public async Task<Bill?> GetBill(long id, bool? withAllocations = false, bool withNoTracking = true, CancellationToken cancellationToken = default)
    {
        var billsQuery = _applicationContext.Bills.WithNoTracking(withNoTracking);

        if (withAllocations == true)
        {
            billsQuery = billsQuery.Include(x => x.BillAllocations);
        }

        return await billsQuery.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task UpdateBill(Bill bill, CancellationToken cancellationToken)
    {
        _applicationContext.Bills.Update(bill);
        await _applicationContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Bill>> GetGroupBills(long billGroupId, bool? withAllocations = false, CancellationToken cancellationToken = default)
    {
        var billQuery = _applicationContext.Bills.WithNoTracking()
            .Where(x => x.BillGroupId == billGroupId && !x.IsDeleted);

        if (withAllocations == true)
        {
            billQuery = billQuery.Include(x => x.BillAllocations)
                .Where(x => !x.IsDeleted);
        }

        return await billQuery.ToListAsync(cancellationToken);
    }
}