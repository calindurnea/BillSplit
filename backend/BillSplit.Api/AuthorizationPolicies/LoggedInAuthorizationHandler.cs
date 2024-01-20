using System.Security.Claims;
using BillSplit.Persistence.Caching;
using Microsoft.AspNetCore.Authorization;

namespace BillSplit.Api.AuthorizationPolicies;

/// <summary>
/// Authorization requirement for logged in users
/// </summary>
public class LoggedInAuthorizationRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Authorization handler to validate that the user is logged in.
/// Makes use of the cache manager to check that the user found
/// in the context has been registered as logged in.
/// </summary>
public class LoggedInAuthorizationHandler : AuthorizationHandler<LoggedInAuthorizationRequirement>
{
    private readonly ICacheManger _cacheManger;

    /// <inheritdoc />
    public LoggedInAuthorizationHandler(ICacheManger cacheManger)
    {
        _cacheManger = cacheManger ?? throw new ArgumentNullException(nameof(cacheManger));
    }

    /// <inheritdoc />
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, LoggedInAuthorizationRequirement requirement)
    {
        var userClaims = context.User.Claims.ToList();
        var userId = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(userId) && await _cacheManger.Exists("authorization:logged:users:" + userId))
        {
            context.Succeed(requirement);
        }
    }
}