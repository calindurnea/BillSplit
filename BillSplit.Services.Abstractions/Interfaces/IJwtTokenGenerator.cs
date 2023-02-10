namespace BillSplit.Services.Abstractions.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(long id);
}
