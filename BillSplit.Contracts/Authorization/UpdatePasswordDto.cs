namespace BillSplit.Contracts.Authorization;

public sealed record UpdatePasswordDto(string Password, string NewPassword, string NewPasswordCheck);
