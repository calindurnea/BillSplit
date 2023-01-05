using System;
using System.Collections.Generic;

namespace BillSplit.Domain.Models;

public partial class User
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long PhoneNumber { get; set; }

    public string? Password { get; set; }

    public bool IsSuperUser { get; set; }

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Bill> BillCreatedByNavigations { get; } = new List<Bill>();

    public virtual ICollection<Bill> BillDeletedByNavigations { get; } = new List<Bill>();

    public virtual ICollection<BillGroup> BillGroupCreatedByNavigations { get; } = new List<BillGroup>();

    public virtual ICollection<BillGroup> BillGroupDeletedByNavigations { get; } = new List<BillGroup>();

    public virtual ICollection<BillGroup> BillGroupUpdatedByNavigations { get; } = new List<BillGroup>();

    public virtual ICollection<Bill> BillUpdatedByNavigations { get; } = new List<Bill>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? DeletedByNavigation { get; set; }

    public virtual ICollection<User> InverseCreatedByNavigation { get; } = new List<User>();

    public virtual ICollection<User> InverseDeletedByNavigation { get; } = new List<User>();

    public virtual ICollection<User> InverseUpdatedByNavigation { get; } = new List<User>();

    public virtual User? UpdatedByNavigation { get; set; }

    public virtual ICollection<UserBillGroup> UserBillGroupCreatedByNavigations { get; } = new List<UserBillGroup>();

    public virtual ICollection<UserBillGroup> UserBillGroupDeletedByNavigations { get; } = new List<UserBillGroup>();

    public virtual ICollection<UserBillGroup> UserBillGroupUsers { get; } = new List<UserBillGroup>();
}
