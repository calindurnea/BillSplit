using System.Text;
using BillSplit.Contracts.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace BillSplit.Domain.Configurations;

public static class TokenValidationConfiguration
{
    public static TokenValidationParameters Get(JwtSettings jwtSettings)
    {
        return new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireAudience = true,
            RequireExpirationTime = true,
            LifetimeValidator = LifetimeValidator
        };
    }


    private static bool LifetimeValidator(DateTime? notbefore, DateTime? expires, SecurityToken securitytoken, TokenValidationParameters validationparameters)
    {
        if (expires != null)
        {
            return expires > DateTime.UtcNow;
        }

        return false;
    }
}