using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class ForbiddenException : Exception
{
    protected ForbiddenException(SerializationInfo info, StreamingContext context)
    {

    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(long id) : base($"You cannot access entity with id: '{id}'")
    {
    }
}