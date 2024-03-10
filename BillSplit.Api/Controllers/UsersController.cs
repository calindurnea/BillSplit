using System.ComponentModel.DataAnnotations;
using System.Net;
using BillSplit.Api.Extensions;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using BillSplit.Domain.Exceptions;
using BillSplit.Domain.ResultHandling;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

/// <inheritdoc />
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Provides functionality for managing users
    /// </summary>
    public UsersController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    /// <summary>
    /// Returns the current user based on the provided authorization
    /// </summary>
    /// <returns>The current user if found, an error otherwise</returns>
    [HttpGet("current", Name = nameof(GetCurrentUser))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> userClaims)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }

        var result = await _userService.GetUser(userClaims.Result.Id);
        return ResultExtensions.HandleResult(result, Ok);
    }

    /// <summary>
    /// Returns a list of all the users
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A list containing all of the users</returns>
    [HttpGet(Name = nameof(GetAllUsers))]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var result = await _userService.GetUsers(cancellationToken);
        return ResultExtensions.HandleResult(result, Ok);
    }

    /// <summary>
    /// Returns the user with the specified id if any is found
    /// </summary>
    /// <param name="id">Id of the user to return</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The user or not found</returns>
    [HttpGet("{id:long}", Name = nameof(GetUserWithId))]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserWithId([FromRoute, BindRequired] long id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUser(id);
        return ResultExtensions.HandleResult(result, Ok);
    }

    /// <summary>
    /// Updates the information of the user
    /// </summary>
    /// <remarks>Only the current user information can be updated</remarks>
    /// <param name="id">User id to update</param>
    /// <param name="upsertUser">User data to update</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpPut("{id:long}", Name = nameof(UpdateUser))]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateUser([FromRoute, BindRequired] long id, [FromBody, Required] UpsertUserDto upsertUser, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }

        if (user.Result.Id != id)
        {
            return ResultExtensions.HandleFailedResult(Result.Failure<UserClaims>("You can only update your own information", HttpStatusCode.Forbidden));
        }

        var result = await _userService.UpdateUser(id, upsertUser);
        return ResultExtensions.HandleResult(result, NoContent());
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <remarks>To fully setup the account call <see cref="AuthorizationController.SetPassword(SetInitialPasswordDto)"/> with the password and user id</remarks>
    /// <param name="upsertUser">User data to create</param>
    /// <param name="cancellationToken"></param>
    [HttpPost(Name = nameof(CreateUser))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] UpsertUserDto upsertUser, CancellationToken cancellationToken)
    {
        var result = await _userService.CreateUser(upsertUser);
        return ResultExtensions.HandleResult(result,
            id => CreatedAtAction(nameof(GetUserWithId),
                new { id },
                new { id }));
    }
}