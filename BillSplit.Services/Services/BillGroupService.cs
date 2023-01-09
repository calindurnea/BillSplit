using BillSplit.Contracts.BillGroup;
using BillSplit.Services.Abstractions.Interfaces;

namespace BillSplit.Services.Services
{
    public class BillGroupService : IBillGroupService
    {
        public Task<IEnumerable<BillGroupDto>> GetByUserId(long userId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}