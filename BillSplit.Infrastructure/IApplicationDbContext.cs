using BillSplit.Domain.Models;
using Microsoft.EntityFrameworkCore;
using User = BillSplit.Domain.Models.User;

namespace BillSplit.Persistence;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Bill> Bills { get; }
    DbSet<BillGroup?> BillGroups { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
