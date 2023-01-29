using BillSplit.Contracts.BillGroup;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillGroupService
{
    Task<BillGroupDto> Get(UserClaims user, long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserBillGroupDto>> Get(UserClaims user, CancellationToken cancellationToken = default);
    Task<long> Create(UserClaims user, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default);
    Task UpdateName(UserClaims user, long id, UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken = default);
    Task RemoveUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default);
    Task AddUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default);
    Task Delete(UserClaims user, long id, CancellationToken cancellationToken = default);
}