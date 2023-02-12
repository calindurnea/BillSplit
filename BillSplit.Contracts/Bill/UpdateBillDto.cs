using BillSplit.Contracts.BillAllocation;

namespace BillSplit.Contracts.Bill;

public sealed class UpdateBillDto
{
    public UpdateBillDto(decimal amount, long paidById, string comment, IEnumerable<CreateBillAllocationDto> billAllocations)
    {
        Amount = decimal.Round(amount, 2);
        PaidById = paidById;
        Comment = comment;
        BillAllocations = billAllocations;
    }

    public decimal Amount { get; }
    public long PaidById { get; }
    public string Comment { get; }
    public IEnumerable<CreateBillAllocationDto> BillAllocations { get; }
}