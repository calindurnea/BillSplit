using BillSplit.Domain.Models;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IJwtTokenGenerator
{
    CreateTokenResult CreateToken(User user);
    string CreateRefreshTokenResult();
}
