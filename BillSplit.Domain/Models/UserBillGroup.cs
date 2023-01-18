namespace BillSplit.Domain.Models;

public partial class UserBillGroup
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long BillGroupId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public long CreatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual BillGroup BillGroup { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
