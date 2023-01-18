namespace BillSplit.Contracts.BillGroup;

public record UpdateBillGroupUsersDto(ISet<long> UserIds);