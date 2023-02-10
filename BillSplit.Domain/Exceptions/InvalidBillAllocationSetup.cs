namespace BillSplit.Domain.Exceptions;

[Serializable]
public class InvalidBillAllocationSetup : Exception
{
    public InvalidBillAllocationSetup(string message) : base(message)
    {
    }
}