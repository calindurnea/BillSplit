using BillSplit.Domain.Models.User;

namespace BillSplit.Controllers;

public interface IUserService
{
    Task<IEnumerable<UserInfo>> Get();
    Task<UserInfo> Get(long id);
    Task<long> Create(CreateUser request);
}