using System.Security.Claims;
using BillSplit.Contracts.User;
using BillSplit.Domain.Models;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IJwtTokenGenerator
{
    AccessTokenResult CreateToken(User user);
    RefreshTokenResult CreateRefreshToken(User user);
    bool TryGetUserClaimsFromRefreshToken(string refreshToken, out UserClaims claims);
    bool TryGetClaimsFromExpiredToken(string accessToken, out ISet<Claim> claims);
}
