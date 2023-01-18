using BillSplit.Contracts.Bill;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces
{
    public interface IBillService
    {
        Task<BillDto> Get(UserClaims user, long id, CancellationToken cancellationToken = default);
    }
}