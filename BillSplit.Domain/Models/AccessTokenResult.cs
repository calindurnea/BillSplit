namespace BillSplit.Domain.Models;

public sealed record AccessTokenResult(string Token, DateTime ExpiresOn);