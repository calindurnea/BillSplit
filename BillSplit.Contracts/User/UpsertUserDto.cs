namespace BillSplit.Contracts.User;

public sealed record UpsertUserDto
{
    public UpsertUserDto(string email, string name, string phoneNumber)
    {
        Email = email.ToLowerInvariant();
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public string Email { get; }
    public string Name { get; }
    public string PhoneNumber { get; }
}
