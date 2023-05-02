﻿using BillSplit.Api.Extensions;
using BillSplit.Contracts.BillGroup;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class BillGroupsController : ControllerBase
{
    private readonly IBillGroupService _billGroupsService;

    public BillGroupsController(IBillGroupService billGroupsService)
    {
        _billGroupsService = billGroupsService ?? throw new ArgumentNullException(nameof(billGroupsService));
    }

    [HttpGet(Name = nameof(GetAllBillGroups))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserBillGroupDto>))]
    public async Task<IActionResult> GetAllBillGroups(CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroups = await _billGroupsService.GetBillGroups(user, cancellationToken);
        return Ok(billGroups);
    }

    [HttpGet("{id:long}", Name = nameof(GetBillGroupById))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BillGroupDto))]
    public async Task<IActionResult> GetBillGroupById([FromRoute, BindRequired] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroup = await _billGroupsService.GetBillGroups(user, id, cancellationToken);
        return Ok(billGroup);
    }

    [HttpPut("{id:long}/add-user", Name = nameof(AddBillGroupUser))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddBillGroupUser([FromRoute, BindRequired] long id, [BindRequired, FromQuery] long userId, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.AddBillGroupUser(user, id, userId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:long}/remove-user", Name = nameof(RemoveBillGroupUser))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveBillGroupUser([FromRoute, BindRequired] long id, [BindRequired, FromQuery] long userId, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.RemoveBillGroupUser(user, id, userId, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:long}/name", Name = nameof(UpdateBillGroupName))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateBillGroupName([FromRoute, BindRequired] long id, [BindRequired, FromBody] UpdateBillGroupNameDto updateBillGroupName, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.UpdateBillGroupName(user, id, updateBillGroupName, cancellationToken);
        return NoContent();
    }

    [HttpPost(Name = nameof(CreateBillGroup))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBillGroup([BindRequired, FromBody] CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var id = await _billGroupsService.CreateBillGroup(user, createBillGroup, cancellationToken);
        return CreatedAtAction(nameof(GetBillGroupById), new { id }, new { id });
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBillGroup([BindRequired, FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billGroupsService.DeleteBillGroup(user, id, cancellationToken);
        return NoContent();
    }
}