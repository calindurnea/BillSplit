using BillSplit.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using IAuthorizationService = BillSplit.Services.Abstractions.Interfaces.IAuthorizationService;

namespace BillSplit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }
    
    [AllowAnonymous]
    [HttpPut("set-password", Name = nameof(SetPassword))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetPassword([FromBody, BindRequired] SetPasswordDto setPassword, CancellationToken cancellationToken = default)
    {
        await _authorizationService.SetPassword(setPassword, cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login", Name = nameof(Login))]
    public async Task<IActionResult> Login([BindRequired] LoginRequestDto loginRequest, CancellationToken cancellationToken = default)
    {
        var response = await _authorizationService.Login(loginRequest, cancellationToken);
        return Ok(response);
    }

    [HttpPost("logout", Name = nameof(Logout))]
    public Task Logout()
    {
        throw new NotImplementedException();
    }
}