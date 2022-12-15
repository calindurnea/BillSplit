namespace BillSplit.Infrastructure.Entities;
internal sealed record UserEntity(
    long Id,
    string Email,
    string Name,
    long PhoneNumber,
    string Password,
    bool IsSuperUser,
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
