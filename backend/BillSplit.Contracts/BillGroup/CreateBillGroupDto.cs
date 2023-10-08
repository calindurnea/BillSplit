namespace BillSplit.Contracts.BillGroup;

public sealed record CreateBillGroupDto(string Name, ISet<long> UserIds);