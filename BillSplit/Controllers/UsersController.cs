using BillSplit.Domain.Models.User;
using BillSplit.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<IEnumerable<UserInfo>> Get()
    {
        // check login?
        return await _userService.Get();
    }

    [HttpGet("{id:long}", Name = nameof(Get))]
    public async Task<UserInfo> Get([FromRoute, BindRequired] long id)
    {
        // check login?
        return await _userService.Get(id);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody, BindRequired] CreateUserRequest request)
    {
        // check login?
        // validate input
        var id = await _userService.Create(new CreateUser(request.Email, request.FirstName, request.LastName, request.PhoneNumber));

        return CreatedAtAction(nameof(Get), new { id = id });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task Login()
    {
        throw new NotImplementedException();
    }

    [HttpPost("logout")]
    public async Task Logout()
    {
        throw new NotImplementedException();
    }

    [HttpPut]
    public async Task Update()
    {
        throw new NotImplementedException();
    }
}
