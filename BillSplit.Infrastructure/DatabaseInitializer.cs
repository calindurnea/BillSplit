using BillSplit.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BillSplit.Persistence;

internal sealed class DbInitializer
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DbInitializer(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    internal async Task Initialize()
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<BillsplitContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        ArgumentNullException.ThrowIfNull(context, nameof(context));
        await context.Database.MigrateAsync();
        if (context.Users.Any()) return;

        var users = new User[]
        {
            new()
            {
                Email = "default@email.com",
                UserName = "default@email.com",
                Name = "Default name",
                PhoneNumber = "1",
                CreatedDate = DateTime.UtcNow
            }
        };

        foreach (var user in users)
        {
            await userManager.CreateAsync(user, "some random password");
        }

        await context.SaveChangesAsync(default);
    }
}