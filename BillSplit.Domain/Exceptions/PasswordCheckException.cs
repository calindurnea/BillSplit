using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class PasswordCheckException : Exception
{
    protected PasswordCheckException(SerializationInfo info, StreamingContext context)
    {

    }

    public PasswordCheckException(string? message) : base(message)
    {
    }
}