using BillSplit.Contracts.BillGroup;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillGroupService
{
    Task<BillGroupDto> Get(UserClaims user, long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserBillGroupDto>> GetByUserId(long userId, CancellationToken cancellationToken = default);
    Task<long> Create(UserClaims user, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default);
}