using BillSplit.Contracts.BillAllocation;

namespace BillSplit.Contracts.Bill
{
    public sealed record BillDto(long Id, long PaidBy, decimal Amount, string Comment, string CreatedByName, DateTime CreatedDate, IEnumerable<BillAllocationDto> BillAllocations);
}