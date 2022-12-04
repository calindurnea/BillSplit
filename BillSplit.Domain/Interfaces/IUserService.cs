using BillSplit.Domain.Models.User;

namespace BillSplit.Domain.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserInfo>> Get();
    Task<UserInfo> Get(long id);
    Task<long> Create(CreateUser request);
}