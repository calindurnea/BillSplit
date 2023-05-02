﻿using System.Globalization;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;
using BillSplit.Contracts.User;

namespace BillSplit.Api.Extensions;

internal static class UserExtensions
{
    internal static UserClaims GetCurrentUser(this IPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is not ClaimsIdentity identity)
        {
            throw new AuthenticationException();
        }

        var userClaims = identity.Claims;
        var id = userClaims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        var email = userClaims.First(x => x.Type == ClaimTypes.Email).Value;

        return new UserClaims(long.Parse(id, CultureInfo.InvariantCulture), email);
    }
}