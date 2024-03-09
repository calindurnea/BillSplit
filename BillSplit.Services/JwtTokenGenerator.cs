using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BillSplit.Contracts.Authorization;
using BillSplit.Domain.Configurations;
using BillSplit.Domain.Models;
using BillSplit.Domain.ResultHandling;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BillSplit.Services;

[SuppressMessage("Performance", "CA1822:Mark members as static")]
internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtTokenGenerator> _logger;

    private const int BearerTokenExpirationMinutes = 10;
    private const int RefreshTokenExpirationMinutes = 30;

    private static readonly Action<ILogger, Exception> ExpiredTokenValidationLogger =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, "ExpiredTokenValidationError"),
            formatString: "Error when validating expired token.");

    private static readonly Action<ILogger, Exception> UnhandledErrorLogger =
        LoggerMessage.Define(
            LogLevel.Critical,
            new EventId(3, "UnhandledError"),
            formatString: "Unhandled error.");
    
    public JwtTokenGenerator(JwtSettings jwtSettings, ILogger<JwtTokenGenerator> logger)
    {
        _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IResult<AccessTokenResult> CreateToken(User user)
    {
        try
        {
            var expiration = DateTime.UtcNow.AddMinutes(BearerTokenExpirationMinutes);

            var claimsResult = CreateClaims(user);

            if (claimsResult is not Result.ISuccessResult<HashSet<Claim>> claims)
            {
                return Result.Failure<AccessTokenResult, HashSet<Claim>>(claimsResult);
            }

            var token = CreateJwtToken(claims.Result, CreateSigningCredentials(), expiration);
            var tokenHandler = new JwtSecurityTokenHandler();
            return Result.Success(new AccessTokenResult(tokenHandler.WriteToken(token), expiration));
        }
        catch (Exception e)
        {
            UnhandledErrorLogger(_logger, e);
            return Result.Failure<AccessTokenResult>("An error occured", HttpStatusCode.InternalServerError);
        }
    }

    public IResult<HashSet<Claim>> TryGetClaimsFromExpiredToken(string accessToken)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(accessToken));
        try
        {
            var tokenValidationParameters = TokenValidationConfiguration.Get(_jwtSettings);
            tokenValidationParameters.LifetimeValidator = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var validatedToken);

            var result = (JwtSecurityToken)validatedToken;
            return Result.Success(result.Claims.ToHashSet());
        }
        catch (SecurityTokenValidationException e)
        {
            ExpiredTokenValidationLogger(_logger, e);
            return Result.Failure<HashSet<Claim>>("An error occured", HttpStatusCode.Unauthorized);
        }
        catch (Exception e)
        {
            UnhandledErrorLogger(_logger, e);
            return Result.Failure<HashSet<Claim>>("An error occured", HttpStatusCode.InternalServerError);
        }
    }

    public IResult<RefreshTokenResult> CreateRefreshToken(User user)
    {
        try
        {
            Debug.Assert(user.Id != 0);
            Debug.Assert(!string.IsNullOrWhiteSpace(user.Email));

            var expiration = DateTime.UtcNow.AddMinutes(RefreshTokenExpirationMinutes).Ticks;
            var bytes = Encoding.UTF8.GetBytes($"{user.Id}:{user.Email}:{expiration}");
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            var combinedData = new byte[bytes.Length + randomNumber.Length];
            Buffer.BlockCopy(bytes, 0, combinedData, 0, bytes.Length);
            Buffer.BlockCopy(randomNumber, 0, combinedData, bytes.Length, randomNumber.Length);

            return Result.Success(new RefreshTokenResult(Convert.ToBase64String(combinedData)));
        }
        catch (Exception e)
        {
            UnhandledErrorLogger(_logger, e);
            return Result.Failure<RefreshTokenResult>("An error occured", HttpStatusCode.InternalServerError);
        }
    }

    public IResult<DeconstructedRefreshToken> DeconstructRefreshToken(string refreshToken)
    {
        Debug.Assert(!string.IsNullOrWhiteSpace(refreshToken));
        try
        {
            var tokenBytes = Convert.FromBase64String(refreshToken);
            var dataBytes = new byte[tokenBytes.Length - 64];
            Buffer.BlockCopy(tokenBytes, 0, dataBytes, 0, dataBytes.Length);
            var decodedData = Encoding.UTF8.GetString(dataBytes);
            var parts = decodedData.Split(':');
            
            Debug.Assert(parts.Length == 3);
            
            var id = long.Parse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture);
            Debug.Assert(id > 0);
            
            var email = parts[1];
            Debug.Assert(!string.IsNullOrWhiteSpace(email));
            
            var expiration = long.Parse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture);
            Debug.Assert(expiration > 0);
            
            return Result.Success(new DeconstructedRefreshToken(id, email, new DateTime(expiration)));
        }
        catch (Exception e)
        {
            UnhandledErrorLogger(_logger, e);
            return Result.Failure<DeconstructedRefreshToken>("An error occured", HttpStatusCode.InternalServerError);
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

    private IResult<HashSet<Claim>> CreateClaims(User user)
    {
        Debug.Assert(user.Id > 0);
        Debug.Assert(!string.IsNullOrWhiteSpace(user.UserName));
        Debug.Assert(!string.IsNullOrWhiteSpace(user.Email));
        
        try
        {
            var issuedAt = DateTimeOffset.UtcNow;
            var iat = (long)issuedAt.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var claims = new HashSet<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, iat.ToString(CultureInfo.InvariantCulture)),
                new(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.Email, user.Email!)
            };
            return Result.Success(claims);
        }
        catch (Exception e)
        {
            UnhandledErrorLogger(_logger, e);
            return Result.Failure<HashSet<Claim>>("An error occured", HttpStatusCode.InternalServerError);
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