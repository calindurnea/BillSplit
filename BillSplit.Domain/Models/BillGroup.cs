using System;
using System.Collections.Generic;

namespace BillSplit.Domain.Models;

public partial class BillGroup
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public long CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public virtual ICollection<Bill> Bills { get; } = new List<Bill>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserBillGroup> UserBillGroups { get; } = new List<UserBillGroup>();
}
