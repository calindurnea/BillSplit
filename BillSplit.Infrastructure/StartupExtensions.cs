using BillSplit.Persistance.Repositories;
using BillSplit.Persistance.Repositories.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BillSplit.Persistance;

public static class StartupExtensions
{
    public static IServiceCollection AddRespositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}
