using BillSplit.Contracts.BillGroup;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillGroupService
{
    Task<IEnumerable<BillGroupDto>> GetByUserId(long userId, CancellationToken cancellationToken = default);
}