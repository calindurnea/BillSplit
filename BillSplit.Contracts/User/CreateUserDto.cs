namespace BillSplit.Contracts.User;

public sealed record CreateUserDto(string Email, string Name, string Password, long PhoneNumber);
