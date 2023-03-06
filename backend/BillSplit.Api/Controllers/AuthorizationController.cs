using BillSplit.Contracts.Authorization;
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
    [HttpPost("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetPassword([FromBody, BindRequired] SetInitialPasswordDto setInitialPassword)
    {
        await _authorizationService.SetInitialPassword(setInitialPassword);
        return NoContent();
    }

    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePassword([FromBody, BindRequired] UpdatePasswordDto updatePassword)
    {
        var user = HttpContext.User;
        await _authorizationService.UpdatePassword(user, updatePassword);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login", Name = nameof(Login))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
    public async Task<IActionResult> Login([BindRequired] LoginRequestDto loginRequest)
    {
        var response = await _authorizationService.Login(loginRequest);
        return Ok(response);
    }

    [HttpPost("logout", Name = nameof(Logout))]
    public Task Logout()
    {
        throw new NotImplementedException();
    }
}