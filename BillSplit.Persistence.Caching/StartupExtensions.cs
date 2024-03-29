﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace BillSplit.Persistence.Caching;

public static class StartupExtensions
{
    public static IServiceCollection AddPersistentCaching(this IServiceCollection services, IConfiguration configuration)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheManger, MemoryCacheManger>();
        }
        else
        {
            var redisConnectionString = configuration.GetConnectionString("RedisConnectionString");

            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new KeyNotFoundException("RedisConnectionString not found");
            }

            services.AddScoped<IDatabaseAsync>(_ =>
                ConnectionMultiplexer.Connect(redisConnectionString, configurationOptions =>
                {
                    configurationOptions.AbortOnConnectFail = true;
                    configurationOptions.Protocol = RedisProtocol.Resp3;
                    configurationOptions.AllowAdmin = false;
                    configurationOptions.IncludePerformanceCountersInExceptions = true;
                }).GetDatabase()
            );
            services.AddScoped<ICacheManger, RedisCacheManger>();
        }

        return services;
    }
}