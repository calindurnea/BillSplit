using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string? message) : base(message)
    {
    }

    public NotFoundException(string name, long userId) : base($"Entity \"{name}\" with id \"{userId}\" was not found")
    {
    }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}