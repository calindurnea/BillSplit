namespace BillSplit.Domain.Entities;

internal sealed record UserBillGroupEntity(
    long UserId,
    long BillGroupId,
    long? CreatedBy,
    long? UpdatedBy,
    long? DeletedBy,
    DateTime? CreatedDate,
    DateTime? UpdatedDate,
    DateTime? DeletedDate)
    : BaseEntity(
        CreatedBy,
        UpdatedBy,
        DeletedBy,
        CreatedDate,
        UpdatedDate,
        DeletedDate);
