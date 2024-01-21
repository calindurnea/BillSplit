using System.ComponentModel.DataAnnotations;

namespace BillSplit.Contracts.Authorization;

public sealed record LoginResponseDto([Required] string Token, [Required] string RefreshToken, [Required] DateTime ExpiresOn);