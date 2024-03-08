using BillSplit.Domain.Models;

namespace BillSplit.Persistence.Repositories.Abstractions;

public interface IUserBillGroupRepository
{
    Task<IEnumerable<long>?> GetUserBillGroupIds(long userId, CancellationToken cancellationToken);
    Task<UserBillGroup?> GetUserBillGroup(long userId, long billGroupId, bool withNoTracking = true, CancellationToken cancellationToken = default);
    Task<IEnumerable<long>> GetBillGroupUserIds(long billGroupId, CancellationToken cancellationToken);
    Task UpdateUserBillGroup(UserBillGroup userBillGroup, CancellationToken cancellationToken);
}