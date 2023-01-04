using System.Runtime.Serialization;

namespace BillSplit.Domain.Exceptions;

[Serializable]
public class PasswordCheckException : Exception
{
    public PasswordCheckException()
    {
    }

    public PasswordCheckException(string? message) : base(message)
    {
    }

    public PasswordCheckException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected PasswordCheckException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}