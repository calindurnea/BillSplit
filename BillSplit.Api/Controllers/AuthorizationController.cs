using System.ComponentModel.DataAnnotations;
using BillSplit.Api.Extensions;
using BillSplit.Contracts.Authorization;
using BillSplit.Contracts.User;
using BillSplit.Domain.ResultHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = BillSplit.Services.Abstractions.Interfaces.IAuthorizationService;

namespace BillSplit.Api.Controllers;

/// <inheritdoc />
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Provides functionality for managing authorization
    /// </summary>
    /// <param name="authorizationService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AuthorizationController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
    }

    /// <summary>
    /// Used to set new password for user after creation
    /// </summary>
    /// <param name="setInitialPassword">Initial password request</param>
    /// <returns>No content if successful, an error otherwise</returns>
    [AllowAnonymous]
    [HttpPost("password", Name = nameof(SetPassword))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetPassword([FromBody, Required] SetInitialPasswordDto setInitialPassword)
    {
        var result = await _authorizationService.SetInitialPassword(setInitialPassword);
        return ResultExtensions.HandleResult(result, NoContent());
    }

    /// <summary>
    /// Updates the password for the user
    /// </summary>
    /// <param name="updatePassword">Password update request</param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpPut("password", Name = nameof(UpdatePassword))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePassword([FromBody, Required] UpdatePasswordDto updatePassword)
    {
        var user = HttpContext.User;
        var result = await _authorizationService.UpdatePassword(user, updatePassword);
        return ResultExtensions.HandleResult(result, NoContent());
    }

    /// <summary>
    /// Generates a token to be used for authorization
    /// </summary>
    /// <param name="loginRequest">Login request</param>
    /// <returns>Bearer token, refresh token and expiration date if login is successful</returns>
    [AllowAnonymous]
    [HttpPost("login", Name = nameof(Login))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
    public async Task<IActionResult> Login([FromBody, Required] LoginRequestDto loginRequest)
    {
        var result = await _authorizationService.Login(loginRequest);
        return ResultExtensions.HandleResult(result, Ok);
    }

    /// <summary>
    /// Generates a new token based on the refresh token
    /// </summary>
    /// <returns>Bearer token, refresh token and expiration date if is successful</returns>
    [AllowAnonymous]
    [HttpPost("refresh", Name = nameof(Refresh))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
    public async Task<IActionResult> Refresh([FromBody, Required] TokenRefreshRequestDto tokenRefreshRequest)
    {
        var result = await _authorizationService.RefreshToken(tokenRefreshRequest);
        return ResultExtensions.HandleResult(result, Ok);
    }

    /// <summary>
    /// Logs out user
    /// </summary>
    [HttpPost("logout", Name = nameof(Logout))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout()
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        var result = await _authorizationService.Logout(user.Result.Id);
        return ResultExtensions.HandleResult(result, NoContent());
    }
}
