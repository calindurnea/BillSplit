namespace BillSplit.Api.Models.Requests;

public sealed record CreateUserRequest(
    string Email,
    string FirstName,
    string LastName,
    long PhoneNumber);
