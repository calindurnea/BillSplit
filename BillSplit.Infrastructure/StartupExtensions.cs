using BillSplit.Persistence.Repositories;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BillSplit.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBillRepository, BillRepository>();
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("ApplicationContext")));
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        return services;
    }
}
