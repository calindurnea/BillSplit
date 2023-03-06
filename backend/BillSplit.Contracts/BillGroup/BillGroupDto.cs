using BillSplit.Contracts.Bill;

namespace BillSplit.Contracts.BillGroup;

public sealed record BillGroupDto(long Id, string Name, IEnumerable<BillDto> Bills);