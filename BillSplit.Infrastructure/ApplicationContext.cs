using BillSplit.Infrastructure.Entities;
using BillSplit.Infrastructure.Entities.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Infrastructure;

internal class ApplicationContext : DbContext
{
    internal DbSet<UserEntity> Users { get; set; }
    //internal DbSet<BillEntity> Bills { get; set; }
    //internal DbSet<BillGroupEntity> BillGroups { get; set; }
    //internal DbSet<UserBillGroup> UserBillGroups { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("billsplit");
        new UserEntityConfiguration().Configure(modelBuilder.Entity<UserEntity>().ToTable("User"));

        base.OnModelCreating(modelBuilder);
    }
}
