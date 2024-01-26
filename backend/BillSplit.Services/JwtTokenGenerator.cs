using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using BillSplit.Domain.Configurations;
using BillSplit.Domain.Models;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BillSplit.Services;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtTokenGenerator> _logger;

    private const int ExpirationMinutes = 1;

    private static readonly Action<ILogger, Exception> ExpiredTokenValidationLogger =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, "ExpiredTokenValidationError"),
            formatString: "Error when validating expired token.");

    private static readonly Action<ILogger, Exception> InvalidRefreshTokenLogger =
        LoggerMessage.Define(
            LogLevel.Critical,
            new EventId(2, "InvalidRefreshTokenError"),
            formatString: "Error when decoding the refresh token.");

    public JwtTokenGenerator(JwtSettings jwtSettings, ILogger<JwtTokenGenerator> logger)
    {
        _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public AccessTokenResult CreateToken(User user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(CreateClaims(user), CreateSigningCredentials(), expiration);
        var tokenHandler = new JwtSecurityTokenHandler();
        return new AccessTokenResult(tokenHandler.WriteToken(token), expiration);
    }

    public bool TryGetClaimsFromExpiredToken(string accessToken, out ISet<Claim> claims)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = TokenValidationConfiguration.Get(_jwtSettings);
            tokenValidationParameters.LifetimeValidator = null;
            tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);

            var result = (JwtSecurityToken)validatedToken;
            claims = result.Claims.ToHashSet();
            return true;
        }
        catch (SecurityTokenValidationException e)
        {
            ExpiredTokenValidationLogger(_logger, e);
            claims = new HashSet<Claim>();
            return false;
        }
    }

    public RefreshTokenResult CreateRefreshToken(User user)
    {
        var bytes = Encoding.UTF8.GetBytes($"{user.Id}:{user.Email}");
        var randomNumber = new byte[64];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        var combinedData = new byte[bytes.Length + randomNumber.Length];
        Buffer.BlockCopy(bytes, 0, combinedData, 0, bytes.Length);
        Buffer.BlockCopy(randomNumber, 0, combinedData, bytes.Length, randomNumber.Length);
        return new RefreshTokenResult(Convert.ToBase64String(combinedData));
    }

    public bool TryGetUserClaimsFromRefreshToken(string refreshToken, out UserClaims claims)
    {
        claims = default!;
        try
        {
            var bytes = Convert.FromBase64String(refreshToken);
            var idEmailBytes = new byte[bytes.Length - 64]; // Extract the array with the id and email bytes
            Buffer.BlockCopy(bytes, 0, idEmailBytes, 0, idEmailBytes.Length);
            var idEmail = Encoding.UTF8.GetString(idEmailBytes); // Convert the bytes back into a string
            var parts = idEmail.Split(':'); // Split the string into the id and email
            var id = long.Parse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture);
            var email = parts[1];
            claims = new UserClaims(id, email);
            return true;
        }
        catch (Exception e)
        {
            InvalidRefreshTokenLogger(_logger, e);
            return false;
        }
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