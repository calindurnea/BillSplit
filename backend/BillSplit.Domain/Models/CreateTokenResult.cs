namespace BillSplit.Domain.Models;

public sealed record CreateTokenResult(string Token, DateTime ExpiresOn);