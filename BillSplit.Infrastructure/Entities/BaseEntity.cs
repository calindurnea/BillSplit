namespace BillSplit.Infrastructure.Entities;

internal record BaseEntity(
    long CreatedBy,
    long UpdatedBy,
    long DeletedBy,
    DateTime CreatedDate,
    DateTime UpdatedDate,
    DateTime DeletedDate);
