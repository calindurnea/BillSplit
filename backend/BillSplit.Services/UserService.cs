using System.Globalization;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Services.Abstractions.Interfaces;
using BillSplit.Services.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BillSplit.Services;

internal sealed class UserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public UserService(UserManager<User> userManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<long> CreateUser(UpsertUserDto request)
    {
        var result = await _userManager.CreateAsync(new User
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            CreatedDate = DateTime.UtcNow
        });

        if (result.Succeeded)
        {
            var user = (await _userManager.FindByEmailAsync(request.Email)).ThrowIfNull();
            return user.Id;
        }

        // TODO: fix this
        throw new Exception(string.Join(", ", result.Errors));
    }

    public async Task UpdateUser(long id, UpsertUserDto request)
    {
        var user = (await _userManager.FindByIdAsync(id.ToString(CultureInfo.InvariantCulture))).ThrowIfNull();

        user.Name = request.Name;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.UpdatedDate = DateTime.UtcNow;
        user.UpdatedBy = id;

        await _userManager.UpdateAsync(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
        return users.Select(MapToDto);
    }

    public async Task<UserDto> GetUser(long id)
    {
        var user = (await _userManager.FindByIdAsync(id.ToString(CultureInfo.InvariantCulture))).ThrowIfNull(id);
        return MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetUsers(ISet<long> ids, CancellationToken cancellationToken = default)
    {
        var users = (await _userManager.Users.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken)).ThrowIfNull();

        if (users.Count != ids.Count)
        {
            var idsNotFound = ids.Where(id => users.Select(u => u.Id).All(uid => uid != id));
            throw new NotFoundException(typeof(User), idsNotFound.ToArray());
        }

        return users.Select(MapToDto);
    }

    private static UserDto MapToDto(User entity)
    {
        return new UserDto(entity.Id, entity.Email!, entity.Name, entity.PhoneNumber!);
    }
}