using BillSplit.Contracts.Authorization;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BillSplit.Services.Extensions;

public static class StartupExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IBillGroupService, BillGroupService>();
        services.AddScoped<IBillService, BillService>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        var jwtSettings = new JwtSettings();
        configuration.Bind("Jwt", jwtSettings);
        services.AddSingleton(jwtSettings);

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }
}
