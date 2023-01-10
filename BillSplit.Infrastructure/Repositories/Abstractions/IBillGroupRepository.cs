using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IBillGroupRepository
{
    Task<IEnumerable<BillGroup>?> GetByUserId(long userId, CancellationToken cancellationToken = default);
    Task<BillGroup?> Get(long id, CancellationToken cancellationToken = default);
    Task<BillGroup> Create(BillGroup billGroup, CancellationToken cancellationToken = default);
}