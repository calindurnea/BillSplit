namespace BillSplit.Domain.Entities;

public sealed record UserEntity(
    long Id,
    string Email,
    string Name,
    long PhoneNumber,
    string Password,
    DateTime CreatedDate,
    DateTime? UpdatedDate = null,
    DateTime? DeletedDate = null,
    bool? IsSuperUser = false,
    long? CreatedBy = null,
    long? UpdatedBy = null,
    long? DeletedBy = null)
{
    public string Email { get; set; } = Email;
    public string Name { get; set; } = Name;
    public long PhoneNumber { get; set; } = PhoneNumber;
    public string Password { get; set; } = Password;
}
