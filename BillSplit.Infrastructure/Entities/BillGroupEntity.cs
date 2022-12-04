namespace BillSplit.Infrastructure.Entities;

internal sealed record BillGroupEntity(
    long Id,
    string Name,
    long CreatedBy,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    DateTime DeletedDate)
    : BaseEntity(CreatedDate, ModifiedDate, DeletedDate);