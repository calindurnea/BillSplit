namespace BillSplit.Contracts.Authorization;

public sealed record SetInitialPasswordDto(long UserId, string Password, string PasswordCheck);
