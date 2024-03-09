using System.Net;
using System.Security.Authentication;
using BillSplit.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BillSplit.Domain.ExceptionHandlers;

public class DefaultExceptionHandler : IExceptionHandler
{
    private readonly ILogger<DefaultExceptionHandler> _logger;

    public DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
#pragma warning disable CA1848
        _logger.LogError(exception, "An error occurred");
#pragma warning restore CA1848

        switch (exception)
        {
            case InvalidBillAllocationSetupException:
            case UnsettledBillAllocationsException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;
                await httpContext.Response.WriteAsJsonAsync(BuildProblemDetails(httpContext, exception, HttpStatusCode.Conflict), cancellationToken);
                break;
            case NotFoundException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await httpContext.Response.WriteAsJsonAsync(BuildProblemDetails(httpContext, exception, HttpStatusCode.NotFound), cancellationToken);
                break;
            case PasswordCheckException:
            case UserCreationException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsJsonAsync(BuildProblemDetails(httpContext, exception, HttpStatusCode.BadRequest), cancellationToken);
                break;
            case ForbiddenException:
            case UnauthorizedAccessException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpContext.Response.WriteAsJsonAsync(BuildProblemDetails(httpContext, exception, HttpStatusCode.Forbidden), cancellationToken);
                break;
            case AuthenticationException:
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpContext.Response.WriteAsJsonAsync(BuildProblemDetails(httpContext, exception, HttpStatusCode.Unauthorized), cancellationToken);
                break;
            default:
                await httpContext.Response.WriteAsJsonAsync(BuildProblemDetails(httpContext, exception, HttpStatusCode.InternalServerError), cancellationToken);
                break;
        }

        return true;
    }

    private static ProblemDetails BuildProblemDetails(HttpContext httpContext, Exception exception, HttpStatusCode statusCodes)
    {
        return new ProblemDetails
        {
            Status = (int)statusCodes,
            Type = exception.GetType().Name,
            Title = "An error occurred",
            Detail = exception.Message,
            Instance = $"{httpContext.Request.Method} {httpContext.Request.Path}"
        };
    }
}