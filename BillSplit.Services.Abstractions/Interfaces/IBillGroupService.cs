using BillSplit.Contracts.BillGroup;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillGroupService
{
    Task<BillGroupDto> GetBillGroups(UserClaims user, long id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserBillGroupDto>> GetBillGroups(UserClaims user, CancellationToken cancellationToken = default);
    Task<long> CreateBillGroup(UserClaims user, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default);
    Task UpdateBillGroupName(UserClaims user, long id, UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken = default);
    Task RemoveBillGroupUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default);
    Task AddBillGroupUser(UserClaims user, long id, long userId, CancellationToken cancellationToken = default);
    Task DeleteBillGroup(UserClaims user, long id, CancellationToken cancellationToken = default);
}