namespace BillSplit.Contracts.BillGroup;

public sealed record UserBillGroupDto(long Id, string Name, decimal TotalAmount, decimal CurrentUserAmount);