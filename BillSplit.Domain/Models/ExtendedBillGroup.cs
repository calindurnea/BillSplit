namespace BillSplit.Domain.Models;

public partial class BillGroup
{
    public BillGroup(string name, long createdBy)
    {
        Name = name;
        CreatedBy = createdBy;
    }
}