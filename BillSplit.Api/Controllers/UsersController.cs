using BillSplit.Contracts.User;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IUserService _userService;

    public UsersController(ILogger<UsersController> logger, IUserService userService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        // check login?
        var users = await _userService.Get(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:long}", Name = nameof(Get))]
    public async Task<UserDto> Get([FromRoute, BindRequired] long id)
    {
        // check login?
        return await _userService.Get(id);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create([FromBody, BindRequired] CreateUserDto createUser)
    {
        // check login?
        // validate input
        var id = await _userService.Create(createUser);

        return CreatedAtAction(nameof(Get), new { id });
    }

    //[AllowAnonymous]
    //[HttpPost("login")]
    //public async Task Login([BindRequired] LoginRequest request)
    //{
    //    throw new NotImplementedException();
    //}

    //[HttpPost("logout")]
    //public async Task Logout()
    //{
    //    throw new NotImplementedException();
    //}

    //[HttpPut]
    //public async Task Update()
    //{
    //    throw new NotImplementedException();
    //}
}

public sealed record LoginRequest(string username, string password);