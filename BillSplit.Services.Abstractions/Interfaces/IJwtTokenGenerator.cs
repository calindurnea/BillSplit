namespace BillSplit.Services.Abstractions.Interfaces;

public interface IJwtTokenGenerator
{
    string Generate(long id);
}
