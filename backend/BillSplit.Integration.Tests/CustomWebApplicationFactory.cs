using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace BillSplit.Integration.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    public CustomWebApplicationFactory()
    {
        var redisContainer = new RedisBuilder()
            .WithEnvironment("REDIS_PASSWORD", "Pass@word")
            .WithEnvironment("REDIS_DATABASES", "1")
            .WithImage("redis:alpine")
            .WithCleanUp(true)
            .WithPortBinding(6379)
            .Build();
        
        var postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("billsplit")
            .WithPassword("billsplit-admin")
            .WithUsername("billsplit-admin")
            .WithImage("postgres:latest")
            .WithCleanUp(true)
            .WithPortBinding(5432)
            .Build();

        redisContainer.StartAsync().Wait();
        postgreSqlContainer.StartAsync().Wait();
    }
}