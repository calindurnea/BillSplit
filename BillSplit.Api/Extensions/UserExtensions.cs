using System.Globalization;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using BillSplit.Contracts.User;
using BillSplit.Domain.ResultHandling;

namespace BillSplit.Api.Extensions;

internal static class UserExtensions
{
    internal static IResult<UserClaims> GetCurrentUserResult(this IPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is not ClaimsIdentity identity)
        {
            return Result.Failure<UserClaims>("Invalid request", HttpStatusCode.Unauthorized);
        }

        var userClaims = identity.Claims.ToList();
        var id = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new AuthenticationException();
        var email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? throw new AuthenticationException();

        return Result.Success(new UserClaims(long.Parse(id, CultureInfo.InvariantCulture), email));
    }
    
    [Obsolete]
    internal static UserClaims GetCurrentUser(this IPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is not ClaimsIdentity identity)
        {
            throw new AuthenticationException();
        }

        var userClaims = identity.Claims.ToList();
        var id = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new AuthenticationException();
        var email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? throw new AuthenticationException();

        return new UserClaims(long.Parse(id, CultureInfo.InvariantCulture), email);
    }
}