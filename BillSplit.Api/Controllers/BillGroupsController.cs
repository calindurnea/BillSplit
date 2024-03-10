using System.ComponentModel.DataAnnotations;
using BillSplit.Api.Extensions;
using BillSplit.Contracts.BillGroup;
using BillSplit.Contracts.User;
using BillSplit.Domain.ResultHandling;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

/// <inheritdoc />
[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
public class BillGroupsController : ControllerBase
{
    private readonly IBillGroupService _billGroupsService;

    /// <summary>
    /// Provides functionality for managing groups
    /// </summary>
    /// <param name="billGroupsService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BillGroupsController(IBillGroupService billGroupsService)
    {
        _billGroupsService = billGroupsService ?? throw new ArgumentNullException(nameof(billGroupsService));
    }

    /// <summary>
    /// Returns all the groups for the user
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of the groups</returns>
    [HttpGet(Name = nameof(GetAllBillGroups))]
    [ProducesResponseType(typeof(IEnumerable<UserBillGroupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBillGroups(CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        var billGroups = await _billGroupsService.GetBillGroups(user.Result, cancellationToken);
        return Ok(billGroups);
    }

    /// <summary>
    /// Returns the group with the specified id if the user has access
    /// </summary>
    /// <param name="id">Id of the group</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The group if successful, an error otherwise</returns>
    [HttpGet("{id:long}", Name = nameof(GetBillGroupById))]
    [ProducesResponseType(typeof(BillGroupDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBillGroupById([FromRoute, BindRequired] long id, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        var result = await _billGroupsService.GetBillGroups(user.Result, id, cancellationToken);
        return ResultExtensions.HandleResult(result, Ok);
    }

    /// <summary>
    /// Adds a user to the group
    /// </summary>
    /// <param name="id">The id of the group</param>
    /// <param name="userId">The id of the user to add</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpPut("{id:long}/add-user", Name = nameof(AddBillGroupUser))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddBillGroupUser([FromRoute, BindRequired] long id, [FromQuery, BindRequired] long userId, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        await _billGroupsService.AddBillGroupUser(user.Result, id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Removes an user from the group if all bills are settled
    /// </summary>
    /// <param name="id">The id of the group</param>
    /// <param name="userId">The id of the user to add</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpPut("{id:long}/remove-user", Name = nameof(RemoveBillGroupUser))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveBillGroupUser([FromRoute, BindRequired] long id, [FromQuery, BindRequired] long userId, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        await _billGroupsService.RemoveBillGroupUser(user.Result, id, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Update the name of a group
    /// </summary>
    /// <param name="id">Id of the group</param>
    /// <param name="updateBillGroupName">New name for the group</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpPut("{id:long}/name", Name = nameof(UpdateBillGroupName))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBillGroupName([FromRoute, BindRequired] long id, [FromBody, Required] UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        await _billGroupsService.UpdateBillGroupName(user.Result, id, updateBillGroupName, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Create a new group
    /// </summary>
    /// <param name="createBillGroup">New group request</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpPost(Name = nameof(CreateBillGroup))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBillGroup([FromBody, Required] CreateBillGroupDto createBillGroup, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        var id = await _billGroupsService.CreateBillGroup(user.Result, createBillGroup, cancellationToken);
        return CreatedAtAction(nameof(GetBillGroupById), new { id }, new { id });
    }

    /// <summary>
    /// Deletes a group if all the bills are settled
    /// </summary>
    /// <param name="id">The id of the group to delete</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpDelete("{id:long}", Name = nameof(DeleteBillGroup))]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBillGroup([FromRoute, BindRequired] long id, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        await _billGroupsService.DeleteBillGroup(user.Result, id, cancellationToken);
        return NoContent();
    }
}