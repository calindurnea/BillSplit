using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IBillGroupRepository
{
    Task<IEnumerable<BillGroup>?> Get(CancellationToken cancellationToken = default, bool withNoTracking = true, params long[] ids);
    Task<BillGroup> Create(BillGroup billGroup, CancellationToken cancellationToken = default);
    Task Update(BillGroup billGroup, CancellationToken cancellationToken = default);
}