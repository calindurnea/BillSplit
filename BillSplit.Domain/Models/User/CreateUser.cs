namespace BillSplit.Domain.Models.User;

public sealed record CreateUser(
    string Email,
    string FirstName,
    string LastName,
    long PhoneNumber);