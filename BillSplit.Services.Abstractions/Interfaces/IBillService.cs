using BillSplit.Contracts.Bill;
using BillSplit.Contracts.User;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IBillService
{
    Task<BillDto> GetBill(UserClaims user, long id, CancellationToken cancellationToken = default);
    Task<long> CreateBill(UserClaims user, CreateBillDto createBill, CancellationToken cancellationToken = default);
}