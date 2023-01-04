using BillSplit.Contracts.Authorization;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BillSplit.Services;

public static class StartupExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();

        var jwtSettings = new JwtSettings();
        configuration.Bind("Jwt", jwtSettings);
        services.AddSingleton(jwtSettings);

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        return services;
    }
}
