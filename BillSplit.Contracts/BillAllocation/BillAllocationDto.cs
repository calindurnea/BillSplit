namespace BillSplit.Contracts.BillAllocation
{
    public sealed record BillAllocationDto(long Id, long UserId, decimal Amount);
}