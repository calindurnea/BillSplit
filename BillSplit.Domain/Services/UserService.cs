using BillSplit.Domain.Interfaces;
using BillSplit.Domain.Models.User;

namespace BillSplit.Domain.Services;

public class UserService : IUserService
{
    public Task<long> Create(CreateUser request)
    {
        throw new NotImplementedException();
    }

    public Task<UserInfo> Get(long id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserInfo>> Get()
    {
        throw new NotImplementedException();
    }
}
