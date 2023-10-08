using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class ForbiddenException : Exception
{
    protected ForbiddenException(SerializationInfo info, StreamingContext context)
    {

    }

    public ForbiddenException(long id) : base($"You cannot access entity with id: {id}")
    {
    }
}