namespace BillSplit.Domain.Exceptions;

[Serializable]
public class UnavailablePhoneNumberException : Exception
{
    public UnavailablePhoneNumberException() : base("Phone number already in use")
    {
    }
}