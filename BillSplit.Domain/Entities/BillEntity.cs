namespace BillSplit.Domain.Entities;

internal sealed record BillEntity(
    long Id,
    long BillGroupId,
    long Amount,
    long CreatedBy,
    long? UpdatedBy,
    long? DeletedBy,
    DateTime CreatedDate,
    DateTime? UpdatedDate,
    DateTime? DeletedDate);
