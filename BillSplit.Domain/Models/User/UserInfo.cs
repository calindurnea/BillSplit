namespace BillSplit.Domain.Models.User;

public sealed record UserInfo
{
    public UserInfo(long id, string email, string firstName, string lastName, long phoneNumber)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    public long Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public long PhoneNumber { get; set; }
}