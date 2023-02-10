using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IBillGroupRepository
{
    Task<IEnumerable<BillGroup>> GetBillGroups(CancellationToken cancellationToken = default, bool withNoTracking = true, params long[] ids);
    Task<BillGroup> CreateBillGroup(BillGroup billGroup, CancellationToken cancellationToken = default);
    Task UpdateBillGroup(BillGroup billGroup, CancellationToken cancellationToken = default);
}