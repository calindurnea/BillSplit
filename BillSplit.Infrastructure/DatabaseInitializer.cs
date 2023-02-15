using BillSplit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence
{
    internal class DbInitializer
    {
        internal static void Initialize(BillsplitContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.Migrate();
            if (dbContext.Users.Any()) return;

            var users = new User[]
            {
                new User("default@user.com", "Default User", 123456789)
            };

            foreach (var user in users)
            {
                dbContext.Users.Add(user);
            }

            dbContext.SaveChanges();
        }
    }
}