using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class InvalidBillAllocationSetupException : Exception
{
    protected InvalidBillAllocationSetupException(SerializationInfo info, StreamingContext context)
    {
        
    }
    
    public InvalidBillAllocationSetupException(string message) : base(message)
    {
    }
}