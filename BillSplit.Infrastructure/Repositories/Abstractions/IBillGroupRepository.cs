using System.Diagnostics.CodeAnalysis;
using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

[SuppressMessage("Design", "CA1068:CancellationToken parameters must come last")]
public interface IBillGroupRepository
{
    Task<IEnumerable<BillGroup>> GetBillGroups(CancellationToken cancellationToken, bool withNoTracking = true, params long[] ids);
    Task<BillGroup> CreateBillGroup(BillGroup billGroup, CancellationToken cancellationToken);
    Task UpdateBillGroup(BillGroup billGroup, CancellationToken cancellationToken);
}