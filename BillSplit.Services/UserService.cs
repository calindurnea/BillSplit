using System.Globalization;
using System.Net;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.Models;
using BillSplit.Domain.ResultHandling;
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

    public async Task<IResult<long>> CreateUser(UpsertUserDto request)
    {
        var result = await _userManager.CreateAsync(new User
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            CreatedDate = DateTime.UtcNow
        });

        if (!result.Succeeded)
        {
            return Result.Failure<long>(
                "User creation failed",
                HttpStatusCode.BadRequest,
                result.Errors.Select(x => x.Description).ToArray());
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Result.Failure<long>("The specified user does not exist", HttpStatusCode.NotFound);
        }

        return Result.Success(user.Id);
    }

    public async Task<IResult<bool>> UpdateUser(long id, UpsertUserDto request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString(CultureInfo.InvariantCulture));

        if (user is null)
        {
            return Result.Failure<bool>("The specified user does not exist", HttpStatusCode.NotFound);
        }
        
        user.Name = request.Name;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber;
        user.UpdatedDate = DateTime.UtcNow;
        user.UpdatedBy = id;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure<bool>(
                "User update failed",
                HttpStatusCode.BadRequest,
                result.Errors.Select(x => x.Description).ToArray());
        }

        return Result.Success(true);
    }

    public async Task<IResult<IEnumerable<UserDto>>> GetUsers(CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.ToListAsync(cancellationToken);
        return Result.Success(users.Select(MapToDto));
    }

    public async Task<IResult<UserDto>> GetUser(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString(CultureInfo.InvariantCulture));

        if (user is null)
        {
            return Result.Failure<UserDto>("The specified user does not exist", HttpStatusCode.NotFound);
        }

        return Result.Success(MapToDto(user));
    }

    public async Task<IResult<IEnumerable<UserDto>>> GetUsers(ISet<long> ids, CancellationToken cancellationToken)
    {
        var users = await _userManager.Users.Where(x => ids.Contains(x.Id)).ToListAsync(cancellationToken);

        if (users.Count == ids.Count)
        {
            return Result.Success(users.Select(MapToDto));
        }

        var idsNotFound = ids.Where(id => users.Select(u => u.Id).All(uid => uid != id));
        return Result.Failure<IEnumerable<UserDto>>(
            "Some of the specified ids could not be found", 
            HttpStatusCode.NotFound,
            idsNotFound.Select(missingId=> $"Missing id: {missingId}").ToArray());
    }

    private static UserDto MapToDto(User entity)
    {
        return new UserDto(entity.Id, entity.Email!, entity.Name, entity.PhoneNumber!);
    }
}