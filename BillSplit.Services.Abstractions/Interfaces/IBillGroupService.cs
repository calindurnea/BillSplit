using BillSplit.Contracts.BillGroup;
using BillSplit.Contracts.User;
using BillSplit.Domain.ResultHandling;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillGroupService
{
    Task<IResult<BillGroupDto>> GetBillGroups(UserClaims user, long id, CancellationToken cancellationToken);
    Task<IEnumerable<UserBillGroupDto>> GetBillGroups(UserClaims user, CancellationToken cancellationToken);
    Task<long> CreateBillGroup(UserClaims user, CreateBillGroupDto createBillGroup, CancellationToken cancellationToken);
    Task UpdateBillGroupName(UserClaims user, long id, UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken);
    Task RemoveBillGroupUser(UserClaims user, long id, long userId, CancellationToken cancellationToken);
    Task AddBillGroupUser(UserClaims user, long id, long userId, CancellationToken cancellationToken);
    Task DeleteBillGroup(UserClaims user, long id, CancellationToken cancellationToken);
}