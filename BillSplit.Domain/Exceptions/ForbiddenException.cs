using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class ForbiddenException : Exception
{
    public ForbiddenException()
    {
    }

    public ForbiddenException(long id) : base($"You cannot access entity with id: {id}")
    {
    }
    
    public ForbiddenException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}