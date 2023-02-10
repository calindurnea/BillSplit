using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BillSplit.Contracts.Authorization;
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

    public string GenerateToken(long id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtSecretKey = Encoding.ASCII.GetBytes(_jwtSettings.Key);
        var claims = new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Name, id.ToString()),
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Subject = claims,
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(500),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtSecretKey), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var serializedToken = tokenHandler.WriteToken(token);

        return serializedToken;
    }
}