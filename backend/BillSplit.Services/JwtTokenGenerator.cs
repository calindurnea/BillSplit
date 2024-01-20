using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BillSplit.Contracts.Authorization;
using BillSplit.Domain.Models;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BillSplit.Services;

internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenGenerator(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
    }

    private const int ExpirationMinutes = 666;

    public CreateTokenResult CreateToken(User user)
    {
        var expiration = DateTime.UtcNow.AddSeconds(30);
        var token = CreateJwtToken(CreateClaims(user), CreateSigningCredentials(), expiration);
        var tokenHandler = new JwtSecurityTokenHandler();
        return new CreateTokenResult(tokenHandler.WriteToken(token), expiration);
    }

    private JwtSecurityToken CreateJwtToken(
        IEnumerable<Claim> claims,
        SigningCredentials credentials,
        DateTime expiration) =>
        new(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private static List<Claim> CreateClaims(User user)
    {
        try
        {
            var issuedAt = DateTimeOffset.UtcNow;
            var iat = (long)issuedAt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, iat.ToString(CultureInfo.InvariantCulture)),
                new(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!)
            };
            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        return new SigningCredentials(
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Key)),
            SecurityAlgorithms.HmacSha256
        );
    }
}