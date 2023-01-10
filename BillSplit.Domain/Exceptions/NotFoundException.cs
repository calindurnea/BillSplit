using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string name) : base($"Entity \"{name}\" was not found")
    {
    }

    public NotFoundException(string name, params long[] ids) : base($"Entity \"{name}\" with ids \"{string.Join(", ", ids)}\" was not found")
    {
    }
    
    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}