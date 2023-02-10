namespace BillSplit.Domain.Exceptions;

public class UnsettledBillAllocationsException : Exception
{
    public UnsettledBillAllocationsException()
        : base("This group cannot be deleted because of unsettled bill allocations")
    {
    }

    public UnsettledBillAllocationsException(string message)
        : base(message)
    {
    }
}