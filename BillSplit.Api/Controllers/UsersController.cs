using BillSplit.Contracts.User;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var users = await _userService.Get(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:long}", Name = nameof(Get))]
    public async Task<IActionResult> Get([FromRoute, BindRequired] long id, CancellationToken cancellationToken = default)
    {
        var user = await _userService.Get(id, cancellationToken);
        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody, BindRequired] CreateUserDto createUser, CancellationToken cancellationToken = default)
    {
        var id = await _userService.Create(createUser, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id });
    }

    [HttpPut("set-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AllowAnonymous]
    public async Task<IActionResult> SetPassword([FromBody, BindRequired] SetPasswordDto setPassword, CancellationToken cancellationToken = default)
    {
        await _userService.SetPassword(setPassword, cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([BindRequired] LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        var response = await _userService.Login(loginRequest, cancellationToken);
        Response.Cookies.Append(JwtBearerDefaults.AuthenticationScheme, response.Token);

        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task Logout()
    {
        throw new NotImplementedException();
    }
}