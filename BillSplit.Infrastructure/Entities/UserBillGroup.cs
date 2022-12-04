namespace BillSplit.Infrastructure.Entities;

internal sealed record UserBillGroup(
    long UserId,
    long BillGroupId,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    DateTime DeletedDate)
    : BaseEntity(CreatedDate, ModifiedDate, DeletedDate);
