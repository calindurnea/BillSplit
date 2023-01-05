using Microsoft.EntityFrameworkCore;
using User = BillSplit.Domain.Models.User;

namespace BillSplit.Persistence;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
