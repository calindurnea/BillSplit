using Microsoft.AspNetCore.Identity;

namespace BillSplit.Domain.Models;

public class User : IdentityUser<long>
{
    public string Name { get; set; } = null!;

    public long? CreatedBy { get; set; }

    public long? UpdatedBy { get; set; }

    public long? DeletedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<BillAllocation> BillAllocationCreatedByNavigations { get; } = new List<BillAllocation>();

    public virtual ICollection<BillAllocation> BillAllocationDeletedByNavigations { get; } = new List<BillAllocation>();

    public virtual ICollection<BillAllocation> BillAllocationUpdatedByNavigations { get; } = new List<BillAllocation>();

    public virtual ICollection<BillAllocation> BillAllocationUsers { get; } = new List<BillAllocation>();

    public virtual ICollection<Bill> BillCreatedByNavigations { get; } = new List<Bill>();

    public virtual ICollection<Bill> BillDeletedByNavigations { get; } = new List<Bill>();

    public virtual ICollection<BillGroup> BillGroupCreatedByNavigations { get; } = new List<BillGroup>();

    public virtual ICollection<BillGroup> BillGroupDeletedByNavigations { get; } = new List<BillGroup>();

    public virtual ICollection<BillGroup> BillGroupUpdatedByNavigations { get; } = new List<BillGroup>();

    public virtual ICollection<Bill> BillPaidByNavigations { get; } = new List<Bill>();

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