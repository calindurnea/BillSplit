using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class UserCreationException : Exception
{
    protected UserCreationException(SerializationInfo info, StreamingContext context)
    {
    }

    public UserCreationException(string message) : base(message)
    {
    }
}