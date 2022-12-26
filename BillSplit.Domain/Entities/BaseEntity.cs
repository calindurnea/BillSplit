namespace BillSplit.Domain.Entities;

public record BaseEntity(
    long? CreatedBy,
    long? UpdatedBy,
    long? DeletedBy,
    DateTime? CreatedDate,
    DateTime? UpdatedDate,
    DateTime? DeletedDate);
