namespace BillSplit.Domain.Entities;
public sealed record UserEntity(
    long Id,
    string Email,
    string Name,
    long PhoneNumber,
    string Password,
    DateTime? CreatedDate = null,
    DateTime? UpdatedDate = null,
    DateTime? DeletedDate = null,
    bool? IsSuperUser = false,
    long? CreatedBy = null,
    long? UpdatedBy = null,
    long? DeletedBy = null)
    : BaseEntity(
        CreatedBy,
        UpdatedBy,
        DeletedBy,
        CreatedDate,
        UpdatedDate,
        DeletedDate);
