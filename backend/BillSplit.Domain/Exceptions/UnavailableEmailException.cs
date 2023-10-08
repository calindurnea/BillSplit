using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class UnavailableEmailException : Exception
{
    protected UnavailableEmailException(SerializationInfo info, StreamingContext context)
    {

    }

    public UnavailableEmailException() : base("Email address already in use")
    {
    }
}