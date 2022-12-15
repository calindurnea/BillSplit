using BillSplit.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Infrastructure;

internal class ApplicationContext : DbContext
{
    internal DbSet<UserEntity> Users { get; set; }
    internal DbSet<BillEntity> Bills { get; set; }
    internal DbSet<BillGroupEntity> BillGroups { get; set; }
    internal DbSet<UserBillGroup> UserBillGroups { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("connectionstring");
}
