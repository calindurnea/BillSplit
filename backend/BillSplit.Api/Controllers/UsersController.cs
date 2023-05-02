using BillSplit.Api.Extensions;
using BillSplit.Contracts.User;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet("current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();

        var users = await _userService.GetUser(user.Id);
        return Ok(users);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetUsers(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:long}", Name = nameof(GetUserWithId))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserWithId([FromRoute, BindRequired] long id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUser(id);
        return Ok(user);
    }

    [HttpPut("{id:long}", Name = nameof(UpdateUser))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateUser([FromRoute, BindRequired] long id, [FromBody, BindRequired] UpsertUserDto upsertUser, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();

        if (user.Id != id)
        {
            return Forbid("You can only update your own data");
        }

        await _userService.UpdateUser(id, upsertUser);
        return NoContent();
    }

    [HttpPost(Name = nameof(CreateUser))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody, BindRequired] UpsertUserDto upsertUser, CancellationToken cancellationToken = default)
    {
        var id = await _userService.CreateUser(upsertUser);
        return CreatedAtAction(nameof(GetUserWithId), new { id }, new { id });
    }
}