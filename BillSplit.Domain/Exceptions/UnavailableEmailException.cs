namespace BillSplit.Domain.Exceptions;

[Serializable]
public class UnavailableEmailException : Exception
{
    public UnavailableEmailException() : base("Email address already in use")
    {
    }
}