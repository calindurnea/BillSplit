﻿namespace BillSplit.Domain.Models;

public partial class UserBillGroup : BaseEntity
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long BillGroupId { get; set; }
    public long CreatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual BillGroup BillGroup { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}
