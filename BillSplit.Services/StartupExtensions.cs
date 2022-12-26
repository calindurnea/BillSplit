using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BillSplit.Services;

public static class StartupExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
