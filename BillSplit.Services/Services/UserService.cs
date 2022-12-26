using BillSplit.Contracts.User;
using BillSplit.Domain.Entities;
using BillSplit.Persistance.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;

namespace BillSplit.Services.Services;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<long> Create(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        // validate if all properties exist
        // validate if email is already in use
        // validate if email is valid email
        // generate storable password
        var result = await _userRepository.Create(new UserEntity(0, request.Email, request.Name, request.PhoneNumber, request.Password));

        return result;
    }

    public async Task<IEnumerable<UserDto>> Get(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.Get(cancellationToken);
        return users.Select(MapToDto);
    }
    public Task<UserDto> Get(long id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    private UserDto MapToDto(UserEntity entity)
    {
        return new UserDto(entity.Id, entity.Email, entity.Name, entity.PhoneNumber);
    }
}
