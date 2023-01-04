namespace BillSplit.Contracts.User;

public sealed record CreateUserDto(string Email, string Name, long PhoneNumber);
