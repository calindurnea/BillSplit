namespace BillSplit.Domain.Models
{
    public partial class Bill
    {
        public Bill()
        {
            
        }
        
        public Bill(decimal amount, string comment, long createdBy, long billGroupId, long paidBy, ICollection<BillAllocation> billAllocations)
        {
            Amount = decimal.Round(amount, 2);
            Comment = comment;
            CreatedBy = createdBy;
            BillGroupId = billGroupId;
            PaidBy = paidBy;
            BillAllocations = billAllocations;
        }
    }
}