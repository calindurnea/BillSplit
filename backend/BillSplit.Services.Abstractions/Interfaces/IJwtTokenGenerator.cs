using System.Security.Claims;
using BillSplit.Domain.Models;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResult CreateToken(User user);
    RefreshTokenResult CreateRefreshToken(User user);
    DeconstructedRefreshToken? DeconstructRefreshToken(string refreshToken);
    bool TryGetClaimsFromExpiredToken(string accessToken, out ISet<Claim> claims);
}
