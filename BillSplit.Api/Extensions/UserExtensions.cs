using System.Security.Claims;
using System.Security.Principal;
using BillSplit.Contracts.User;

namespace BillSplit.Api.Extensions;

internal static class UserExtensions
{
    internal static UserClaims? GetCurrentUser(this IPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is not ClaimsIdentity identity)
        {
            return null;
        }

        var userClaims = identity.Claims;
        var id = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

        return !long.TryParse(id, out var parsedId) ? null : new UserClaims(parsedId);
    }
}