using BillSplit.Domain.Models.User;

namespace BillSplit.Domain.Models.BillGroup;

public sealed record BillGroup(
    long Id, 
    string Name,
    UserInfo CreatedBy,
    DateTime CreatedDate,
    DateTime ModifiedDate,
    DateTime DeletedDate);