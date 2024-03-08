using BillSplit.Domain.Models;
using BillSplit.Persistence.Extensions;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories;

internal sealed class UserBillGroupRepository : IUserBillGroupRepository
{
    private readonly IApplicationDbContext _applicationContext;

    public UserBillGroupRepository(IApplicationDbContext applicationContext)
    {
        _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    public async Task<IEnumerable<long>?> GetUserBillGroupIds(long userId, CancellationToken cancellationToken)
    {
        return await _applicationContext.UserBillGroups.WithNoTracking()
            .Where(x => x.UserId == userId && !x.IsDeleted)
            .Select(x => x.BillGroupId)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserBillGroup?> GetUserBillGroup(long userId, long billGroupId, bool withNoTracking = true, CancellationToken cancellationToken = default)
    {
        return await _applicationContext.UserBillGroups.WithNoTracking(withNoTracking)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.BillGroupId == billGroupId &&
                !x.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<long>> GetBillGroupUserIds(long billGroupId, CancellationToken cancellationToken)
    {
        return await _applicationContext.UserBillGroups.WithNoTracking()
            .Where(x => x.BillGroupId == billGroupId && !x.IsDeleted)
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateUserBillGroup(UserBillGroup userBillGroup, CancellationToken cancellationToken)
    {
        _applicationContext.UserBillGroups.Update(userBillGroup);
        await _applicationContext.SaveChangesAsync(cancellationToken);
    }
}