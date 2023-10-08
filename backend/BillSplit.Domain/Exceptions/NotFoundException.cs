using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class NotFoundException : Exception
{
    protected NotFoundException(SerializationInfo info, StreamingContext context)
    {

    }

    public NotFoundException(string name) : base($"Entity \"{name}\" was not found")
    {
    }

    public NotFoundException(Type name, params long[] ids) : base($"Entity \"{name}\" with ids \"{string.Join(", ", ids)}\" was not found")
    {
    }
}