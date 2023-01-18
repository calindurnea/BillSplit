using BillSplit.Contracts.BillAllocation;

namespace BillSplit.Contracts.Bill
{
    public sealed record BillDto(long Id, long PaidBy, string PaidByName, decimal Amount, string Comment, string CreatedByName, DateTime CreatedDate, IEnumerable<BillAllocationDto> BillAllocations);
}