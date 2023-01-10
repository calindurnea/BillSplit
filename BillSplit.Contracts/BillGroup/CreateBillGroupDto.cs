namespace BillSplit.Contracts.BillGroup;

public record CreateBillGroupDto(string Name, ISet<long> UserIds);