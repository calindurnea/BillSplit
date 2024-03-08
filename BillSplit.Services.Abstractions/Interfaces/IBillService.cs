using BillSplit.Contracts.Bill;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillService
{
    Task<BillDto> GetBill(UserClaims user, long id, CancellationToken cancellationToken);
    Task<long> UpsertBill(UserClaims user, UpsertBillDto upsertBill, CancellationToken cancellationToken);
    Task Delete(UserClaims user, long id, CancellationToken cancellationToken);
}