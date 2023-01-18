using BillSplit.Domain.Models;
using BillSplit.Persistence.Extensions;
using BillSplit.Persistence.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Repositories
{
    internal class UserBillGroupRepository : IUserBillGroupRepository
    {
        private readonly IApplicationDbContext _applicationContext;

        public UserBillGroupRepository(IApplicationDbContext applicationContext)
        {
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        public async Task<IEnumerable<long>?> GetUserBillGroupIds(long userId, CancellationToken cancellationToken = default)
        {
            return await _applicationContext.UserBillGroups.WithNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .Select(x => x.BillGroupId)
                .ToListAsync(cancellationToken);
        }

        public async Task<UserBillGroup?> Get(long userId, long billGroupId, bool withNoTracking = true, CancellationToken cancellationToken = default)
        {
            return await _applicationContext.UserBillGroups.WithNoTracking(withNoTracking)
                .FirstOrDefaultAsync(x =>
                        x.UserId == userId &&
                        x.BillGroupId == billGroupId &&
                        !x.IsDeleted, cancellationToken);
        }

        public async Task Update(UserBillGroup userBillGroup, CancellationToken cancellationToken = default)
        {
            _applicationContext.UserBillGroups.Update(userBillGroup);
            await _applicationContext.SaveChangesAsync(cancellationToken);
        }
    }
}