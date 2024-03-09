using System.Security.Claims;
using BillSplit.Domain.Models;
using BillSplit.Domain.ResultHandling;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IJwtTokenGenerator
{
    IResult<AccessTokenResult> CreateToken(User user);
    IResult<RefreshTokenResult> CreateRefreshToken(User user);
    IResult<DeconstructedRefreshToken> DeconstructRefreshToken(string refreshToken);
    IResult<HashSet<Claim>> TryGetClaimsFromExpiredToken(string accessToken);
}
