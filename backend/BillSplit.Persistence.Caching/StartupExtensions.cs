using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BillSplit.Persistence.Caching;

public static class StartupExtensions
{
    public static IServiceCollection AddPersistentCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");

        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new KeyNotFoundException("RedisConnectionString not found");
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "bill-split";
        });

        services.AddScoped<IDatabase>(_ => ConnectionMultiplexer.Connect(redisConnectionString).GetDatabase());
        services.AddScoped<ICacheManger, CacheManger>();
        return services;
    }
}
