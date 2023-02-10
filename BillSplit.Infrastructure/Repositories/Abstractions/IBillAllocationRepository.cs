using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IBillAllocationRepository
{
    Task<IEnumerable<BillAllocation>> GetBillsAllocations(IEnumerable<long> billIds, CancellationToken cancellationToken = default);
    Task<IEnumerable<BillAllocation>> GetUserBillGroupAllocations(long userId, long billGroupId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BillAllocation>> GetBillGroupAllocations(long billGroupId, CancellationToken cancellationToken = default);
}