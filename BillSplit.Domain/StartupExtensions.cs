using System.Net;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using BillSplit.Domain.ExceptionHandlers;
using Microsoft.AspNetCore.Mvc;

namespace BillSplit.Domain;

public static class StartupExtensions
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddExceptionHandler<DefaultExceptionHandler>();
        services.AddControllers().ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "One or more validation errors occurred.",
                    Extensions = new Dictionary<string, object?>
                    {
                        {"reasons", errors}
                    }
                };
                return new BadRequestObjectResult(problemDetails);
            };
        });
        return services;
    }
}