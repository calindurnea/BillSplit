namespace BillSplit.Domain.Models;

public sealed record DeconstructedRefreshToken(long Id, string Email, DateTime Expiry);