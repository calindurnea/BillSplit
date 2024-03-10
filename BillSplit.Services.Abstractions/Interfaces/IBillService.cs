using BillSplit.Contracts.Bill;
using BillSplit.Contracts.User;
using BillSplit.Domain.ResultHandling;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillService
{
    Task<IResult<BillDto>> GetBill(UserClaims user, long id, CancellationToken cancellationToken);
    Task<long> UpsertBill(UserClaims user, UpsertBillDto upsertBill, CancellationToken cancellationToken);
    Task Delete(UserClaims user, long id, CancellationToken cancellationToken);
}