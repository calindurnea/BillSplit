namespace BillSplit.Contracts.BillGroup
{
    public record BillGroupDto(long Id, string Name, decimal TotalAmount, decimal CurrentUserAmount);
}