using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class UnavailableEmailException : Exception
{
    public UnavailableEmailException()
    {
    }

    public UnavailableEmailException(string? message) : base(message)
    {
    }

    public UnavailableEmailException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected UnavailableEmailException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}