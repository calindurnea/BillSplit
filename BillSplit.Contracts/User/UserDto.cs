namespace BillSplit.Contracts.User;

public sealed record UserDto
{
    public UserDto(long id, string email, string name, long phoneNumber)
    {
        Id = id;
        Email = email;
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public long Id { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public long PhoneNumber { get; set; }
}