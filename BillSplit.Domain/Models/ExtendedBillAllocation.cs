namespace BillSplit.Domain.Models;

public partial class BillAllocation
{
    public BillAllocation(long userId, decimal amount, long createdBy)
    {
        UserId = userId;
        Amount = decimal.Round(amount, 2);
        CreatedBy = createdBy;
        PaidAmount = 0;
    }
}