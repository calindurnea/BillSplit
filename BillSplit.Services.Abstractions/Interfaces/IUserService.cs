using BillSplit.Contracts.User;
using BillSplit.Domain.ResultHandling;

namespace BillSplit.Services.Abstractions.Interfaces;

public interface IUserService
{
    Task<IResult<IEnumerable<UserDto>>> GetUsers(CancellationToken cancellationToken);
    Task<IResult<UserDto>> GetUser(long id);
    Task<IResult<IEnumerable<UserDto>>> GetUsers(ISet<long> ids, CancellationToken cancellationToken);
    Task<IResult<long>> CreateUser(UpsertUserDto request);
    Task<IResult<bool>> UpdateUser(long id, UpsertUserDto request);
}