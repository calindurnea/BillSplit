namespace BillSplit.Domain.Models;

public partial class User
{
    public User(string email, string name, long phoneNumber)
    {
        Email = email;
        Name = name;
        PhoneNumber = phoneNumber;
    }
}