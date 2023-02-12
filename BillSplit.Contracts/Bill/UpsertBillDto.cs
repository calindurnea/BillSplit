using BillSplit.Contracts.BillAllocation;

namespace BillSplit.Contracts.Bill;

public sealed class UpsertBillDto
{
    public UpsertBillDto(long? id, decimal amount, long billGroupId, long paidById, string comment, IEnumerable<CreateBillAllocationDto> billAllocations)
    {
        Id = id;
        Amount = decimal.Round(amount, 2);
        BillGroupId = billGroupId;
        PaidById = paidById;
        Comment = comment;
        BillAllocations = billAllocations;
    }

    public long? Id { get; }
    public decimal Amount { get; }
    public long BillGroupId { get; }
    public long PaidById { get; }
    public string Comment { get; }
    public IEnumerable<CreateBillAllocationDto> BillAllocations { get; }
}