using BillSplit.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence;

public interface IApplicationDbContext
{
    DbSet<Bill> Bills { get; }
    DbSet<BillGroup> BillGroups { get; }
    DbSet<UserBillGroup> UserBillGroups { get; }
    DbSet<BillAllocation> BillAllocations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}