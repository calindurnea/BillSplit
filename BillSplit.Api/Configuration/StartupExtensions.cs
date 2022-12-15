using BillSplit.Domain.Interfaces;
using BillSplit.Domain.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;

namespace BillSplit.Api.Configuration;

public static class ServiceCollectionExtensions
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