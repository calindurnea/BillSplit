using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

public class UnsettledBillAllocationsException : Exception
{
    protected UnsettledBillAllocationsException(SerializationInfo info, StreamingContext context)
    {
        
    }
    
    public UnsettledBillAllocationsException(string message)
        : base(message)
    {
    }
}