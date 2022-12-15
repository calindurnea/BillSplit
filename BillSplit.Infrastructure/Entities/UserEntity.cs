namespace BillSplit.Infrastructure.Entities;

internal sealed record UserEntity(
    long Id,
    string Email,
    string Name,
    long PhoneNumber,
    string Password,
    bool IsSuperUser,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    DateTime DeletedDate)
    : BaseEntity(CreatedDate, ModifiedDate, DeletedDate);