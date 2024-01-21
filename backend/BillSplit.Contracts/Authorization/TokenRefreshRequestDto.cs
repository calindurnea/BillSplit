namespace BillSplit.Contracts.Authorization;

public sealed record TokenRefreshRequestDto(string Token, string RefreshToken);