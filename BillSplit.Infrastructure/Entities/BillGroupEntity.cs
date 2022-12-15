namespace BillSplit.Infrastructure.Entities;

internal sealed record BillGroupEntity(
    long Id,
    string Name,
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