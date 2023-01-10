namespace BillSplit.Contracts.User;

public sealed record UpsertUserDto
{
    public UpsertUserDto(string email, string name, long phoneNumber)
    {
        Email = email.ToLowerInvariant();
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public string Email { get; init; }
    public string Name { get; init; }
    public long PhoneNumber { get; init; }
}
