using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IBillRepository
{
    Task<Bill> Create(Bill bill, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> Get(CancellationToken cancellationToken = default);
    Task<Bill?> Get(long id, CancellationToken cancellationToken = default);
    Task Update(Bill bill, CancellationToken cancellationToken = default);
}