namespace BillSplit.Domain.Models;

public partial class UserBillGroup
{
    public UserBillGroup(long userId, long createdBy)
    {
        UserId = userId;
        CreatedBy = createdBy;
    }
}
