namespace BillSplit.Domain.Models;

public partial class BillSplit
{
    public long Id { get; set; }

    public long BillId { get; set; }

    public long UserId { get; set; }

    public decimal Amount { get; set; }

    public virtual Bill Bill { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
