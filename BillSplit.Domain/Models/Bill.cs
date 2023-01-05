using System;
using System.Collections.Generic;

namespace BillSplit.Domain.Models;

public partial class Bill
{
    public long Id { get; set; }

    public decimal Amount { get; set; }

    public string Comment { get; set; } = null!;

    public long CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public long BillGroupId { get; set; }

    public virtual BillGroup BillGroup { get; set; } = null!;

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual User? DeletedByNavigation { get; set; }

    public virtual User? UpdatedByNavigation { get; set; }
}
