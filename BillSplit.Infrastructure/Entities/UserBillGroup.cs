namespace BillSplit.Infrastructure.Entities;

internal sealed record UserBillGroup(
    long UserId,
    long BillGroupId,
    long CreatedBy,
    long UpdatedBy,
    long DeletedBy,
    DateTime CreatedDate,
    DateTime UpdatedDate,
    DateTime DeletedDate)
    : BaseEntity(
        CreatedBy,
        UpdatedBy,
        DeletedBy,
        CreatedDate,
        UpdatedDate,
        DeletedDate);
