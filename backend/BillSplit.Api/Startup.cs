using System.Security.Claims;
using BillSplit.Api.AuthorizationPolicies;
using BillSplit.Api.Extensions;
using BillSplit.Domain;
using BillSplit.Persistence;
using BillSplit.Persistence.Caching;
using BillSplit.Persistence.Extensions;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace BillSplit.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options => { options.Filters.Add(new ProducesAttribute("application/json")); });

        services.AddScoped<IAuthorizationHandler, LoggedInAuthorizationHandler>();

        services.AddEndpointsApiExplorer()
            .ConfigureSwagger()
            .AddInfrastructure(Configuration)
            .AddServices(Configuration)
            .AddRepositories()
            .AddValidators()
            // .AddOutputCache();
            .AddPersistentCaching(Configuration)
            .ConfigureAuthentication(Configuration)
            .AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .AddRequirements(new LoggedInAuthorizationRequirement())
                    .RequireAuthenticatedUser()
                    .RequireClaim(ClaimTypes.Email)
                    .RequireClaim(ClaimTypes.NameIdentifier)
                    .RequireClaim(ClaimTypes.Name)
                    .Build();
            });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // if (env.IsDevelopment())
        // {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "swagger";
            options.DisplayOperationId();
            options.DisplayRequestDuration();
        });

        app.UseDeveloperExceptionPage();
        app.SeedData();
        // }
        // else
        // {
        //     app.UseExceptionHandler("/Error");
        //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        //     app.UseHsts();
        // }

        app.UseExceptionHandler(options => { });
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        // app.UseOutputCache();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(o => { o.MapControllers().RequireAuthorization(); });
    }
}