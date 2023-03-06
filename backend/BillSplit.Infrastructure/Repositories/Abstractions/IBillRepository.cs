using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IBillRepository
{
    Task<Bill> CreateBill(Bill bill, CancellationToken cancellationToken = default);
    Task<Bill?> GetBill(long id, bool? withAllocations = false, bool withNoTracking = true, CancellationToken cancellationToken = default);
    Task UpdateBill(Bill bill, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetGroupBills(long billGroupId, bool? withAllocations = false, CancellationToken cancellationToken = default);
}