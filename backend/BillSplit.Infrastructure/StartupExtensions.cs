using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BillSplit.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBillRepository, BillRepository>();
        services.AddScoped<IBillGroupRepository, BillGroupRepository>();
        services.AddScoped<IBillAllocationRepository, BillAllocationRepository>();
        services.AddScoped<IUserBillGroupRepository, UserBillGroupRepository>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillsplitContext>(options => options.UseNpgsql(configuration.GetConnectionString("ApplicationContext"),
            x => x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "billsplit")));
        services.AddScoped<IApplicationDbContext, BillsplitContext>();

        services.AddSingleton<DbInitializer>();

        services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            }).AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<BillsplitContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IApplicationBuilder SeedData(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app, nameof(app));

        var dbInitializer = app.ApplicationServices.GetRequiredService<DbInitializer>();
        
        var initializationTask = Task.Run(async () => await dbInitializer.Initialize());
        initializationTask.Wait();

        Console.WriteLine($"Initialization completed successfully: {initializationTask.IsCompletedSuccessfully}");
        
        return app;
    }
}