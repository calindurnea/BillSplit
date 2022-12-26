using BillSplit.Domain.Entities;
using BillSplit.Persistance.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistance;

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<UserEntity> Users { get; set; }
    //internal DbSet<BillEntity> Bills { get; set; }
    //internal DbSet<BillGroupEntity> BillGroups { get; set; }
    //internal DbSet<UserBillGroup> UserBillGroups { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("billsplit");
        new UserEntityConfiguration().Configure(modelBuilder.Entity<UserEntity>().ToTable("User"));

        base.OnModelCreating(modelBuilder);
    }
}
