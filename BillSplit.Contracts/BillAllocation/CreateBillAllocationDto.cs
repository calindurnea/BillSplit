namespace BillSplit.Contracts.BillAllocation;

public sealed class CreateBillAllocationDto
{
    public CreateBillAllocationDto(long userId, decimal amount)
    {
        UserId = userId;
        Amount = decimal.Round(amount, 2);
    }

    public long UserId { get; }
    public decimal Amount { get; }
}