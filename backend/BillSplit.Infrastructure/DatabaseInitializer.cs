using BillSplit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence
{
    internal sealed class DbInitializer
    {
        internal static void Initialize(BillsplitContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.Migrate();
            if (dbContext.Users.Any()) return;

            var users = new User[]
            {
                new()
                {
                    Email = "default@email.com",
                    NormalizedEmail = "default@email.com",
                    Name = "Default name",
                    PhoneNumber = "1"
                }
            };

            foreach (var user in users)
            {
                dbContext.Users.Add(user);
            }

            dbContext.SaveChanges();
        }
    }
}