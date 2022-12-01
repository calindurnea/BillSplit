using BillSplit.Controllers;
using BillSplit.Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace BillSplit.Configuration;

public static class StartupExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}
