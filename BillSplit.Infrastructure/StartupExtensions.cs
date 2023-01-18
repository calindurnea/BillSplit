﻿using BillSplit.Persistence.Repositories;
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
        services.AddScoped<IBillGroupRepository, BillGroupRepository>();
        services.AddScoped<IBillAllocationRepository, BillAllocationRepository>();
        services.AddScoped<IUserBillGroupRepository, UserBillGroupRepository>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BillsplitContext>(options => options.UseNpgsql(configuration.GetConnectionString("ApplicationContext")));
        services.AddScoped<IApplicationDbContext, BillsplitContext>();
        return services;
    }
}