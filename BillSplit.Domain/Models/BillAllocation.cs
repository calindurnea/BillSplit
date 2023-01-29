namespace BillSplit.Domain.Models;

public partial class BillAllocation
{
    public long Id { get; set; }

    public long BillId { get; set; }

    public long UserId { get; set; }

    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }

    public long CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
