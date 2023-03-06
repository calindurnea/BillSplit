using BillSplit.Domain.Models;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IJwtTokenGenerator
{
    string CreateToken(User user);
}
