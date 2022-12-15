namespace BillSplit.Infrastructure.Entities;

internal sealed record BillEntity(
    long Id,
    long BillGroupId,
    long Amount,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    DateTime DeletedDate)
    : BaseEntity(CreatedDate, ModifiedDate, DeletedDate);
