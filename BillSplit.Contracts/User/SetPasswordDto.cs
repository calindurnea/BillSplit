namespace BillSplit.Contracts.User;

public sealed record SetPasswordDto(long UserId, string Password, string PasswordCheck);
