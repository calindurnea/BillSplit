using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Persistence.Repositories.Abstractions;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;

namespace BillSplit.Services;

internal sealed class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<long> CreateUser(UpsertUserDto request, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.IsEmailInUse(request.Email.ToLowerInvariant(), cancellationToken))
        {
            throw new UnavailableEmailException();
        }

        if (await _userRepository.IsPhoneNumberInUse(request.PhoneNumber, cancellationToken))
        {
            throw new UnavailablePhoneNumberException();
        }

        var result = await _userRepository.CreateUser(new User(request.Email, request.Name, request.PhoneNumber), cancellationToken);

        return result.Id;
    }

    public async Task UpdateUser(long id, UpsertUserDto request, CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.GetUsers(id, cancellationToken)).ThrowIfNull(id);

        if (!string.Equals(user.Email.ToLowerInvariant(), request.Email, StringComparison.Ordinal) &&
            await _userRepository.IsEmailInUse(request.Email, cancellationToken))
        {
            throw new UnavailableEmailException();
        }

        user.Email = request.Email;

        if (user.PhoneNumber != request.PhoneNumber &&
            await _userRepository.IsPhoneNumberInUse(request.PhoneNumber, cancellationToken))
        {
            throw new UnavailablePhoneNumberException();
        }

        user.PhoneNumber = request.PhoneNumber;
        user.Name = request.Name;

        await _userRepository.UpdateUser(user, cancellationToken);
    }

    public async Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetUsers(cancellationToken);
        return users.Select(MapToDto);
    }

    public async Task<UserDto> GetUsers(long id, CancellationToken cancellationToken = default)
    {
        var user = (await _userRepository.GetUsers(id, cancellationToken)).ThrowIfNull(id);
        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsers(IEnumerable<long> ids, CancellationToken cancellationToken = default)
    {
        ids = ids.ToList();

        var users = (await _userRepository.GetUsers(ids, cancellationToken))
            .ThrowIfNull(ids.ToArray())
            .ToList();

        if (users.Count != ids.Count())
        {
            var idsNotFound = ids.Where(id => users.Select(u => u.Id).All(uid => uid != id));
            throw new NotFoundException(typeof(User), idsNotFound.ToArray());
        }

        return users.Select(MapToDto);
    }

    private static UserDto MapToDto(User entity)
    {
        return new UserDto(entity.Id, entity.Email, entity.Name, entity.PhoneNumber);
    }
}