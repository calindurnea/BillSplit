using BillSplit.Api.Extensions;
using BillSplit.Contracts.BillGroup;
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
    public async Task<IActionResult> GetAllBillGroups(CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroups = await _billGroupsService.GetBillGroups(user, cancellationToken);
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
    public async Task<IActionResult> GetBillGroupById([FromRoute, BindRequired] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroup = await _billGroupsService.GetBillGroups(user, id, cancellationToken);
        return Ok(billGroup);
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
    public async Task<IActionResult> AddBillGroupUser([FromRoute, BindRequired] long id, [BindRequired, FromQuery] long userId, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.AddBillGroupUser(user, id, userId, cancellationToken);
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
    public async Task<IActionResult> RemoveBillGroupUser([FromRoute, BindRequired] long id, [BindRequired, FromQuery] long userId, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.RemoveBillGroupUser(user, id, userId, cancellationToken);
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
    public async Task<IActionResult> UpdateBillGroupName([FromRoute, BindRequired] long id, [BindRequired, FromBody] UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.UpdateBillGroupName(user, id, updateBillGroupName, cancellationToken);
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
    public async Task<IActionResult> CreateBillGroup([BindRequired, FromBody] CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var id = await _billGroupsService.CreateBillGroup(user, createBillGroup, cancellationToken);
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
    public async Task<IActionResult> DeleteBillGroup([BindRequired, FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.DeleteBillGroup(user, id, cancellationToken);
        return NoContent();
    }
}