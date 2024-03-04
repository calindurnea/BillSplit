using System.Security.Claims;
using BillSplit.Api.AuthorizationPolicies;
using BillSplit.Api.Extensions;
using BillSplit.Domain;
using BillSplit.Persistence.Caching;
using BillSplit.Persistence.Extensions;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => { options.Filters.Add(new ProducesAttribute("application/json")); });

builder.Services.AddScoped<IAuthorizationHandler, LoggedInAuthorizationHandler>();

builder.Services.AddEndpointsApiExplorer()
    .ConfigureSwagger()
    .AddInfrastructure(builder.Configuration)
    .AddServices(builder.Configuration)
    .AddRepositories()
    .AddValidators()
    .AddPersistentCaching(builder.Configuration)
    .ConfigureAuthentication(builder.Configuration)
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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
    options.DisplayOperationId();
    options.DisplayRequestDuration();
});

app.SeedData();

app.UseExceptionHandler(_ => { });
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireAuthorization();

app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
// Required for integration testing
namespace BillSplit.Api
{
    public partial class Program
    {
    }
}