namespace BillSplit.Domain.Models;

public partial class Bill : BaseEntity
{
    public long Id { get; set; }

    public decimal Amount { get; set; }

    public string Comment { get; set; } = null!;

    public long CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public long BillGroupId { get; set; }

    public long PaidBy { get; set; }

    public virtual ICollection<BillAllocation> BillAllocations { get; } = new List<BillAllocation>();

    public virtual BillGroup BillGroup { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User PaidByNavigation { get; set; } = null!;

    public virtual User? UpdatedByNavigation { get; set; }
}
