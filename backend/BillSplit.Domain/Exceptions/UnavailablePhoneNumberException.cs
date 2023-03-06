using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class UnavailablePhoneNumberException : Exception
{
    protected UnavailablePhoneNumberException(SerializationInfo info, StreamingContext context)
    {
        
    }
    
    public UnavailablePhoneNumberException() : base("Phone number already in use")
    {
    }
}