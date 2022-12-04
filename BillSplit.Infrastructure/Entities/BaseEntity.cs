namespace BillSplit.Infrastructure.Entities;

internal record BaseEntity(
    DateTime CreatedDate,
    DateTime ModifiedDate,
    DateTime DeletedDate);
