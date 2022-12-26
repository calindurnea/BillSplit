using BillSplit.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistance;

public interface IApplicationDbContext
{
    DbSet<UserEntity> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}
