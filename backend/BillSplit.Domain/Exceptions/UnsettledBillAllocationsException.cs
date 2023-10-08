namespace BillSplit.Domain.Exceptions;

public class UnsettledBillAllocationsException : Exception
{
    public UnsettledBillAllocationsException(string message)
        : base(message)
    {
    }
}