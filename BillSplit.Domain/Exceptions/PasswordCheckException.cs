namespace BillSplit.Domain.Exceptions;

[Serializable]
public class PasswordCheckException : Exception
{
    public PasswordCheckException(string? message) : base(message)
    {
    }
}