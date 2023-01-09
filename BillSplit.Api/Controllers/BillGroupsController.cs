using BillSplit.Api.Extensions;
using BillSplit.Contracts.BillGroup;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillGroupsController : ControllerBase
{
    private readonly IBillGroupService _billGroupsService;

    public BillGroupsController(IBillGroupService billGroupsService)
    {
        _billGroupsService = billGroupsService ?? throw new ArgumentNullException(nameof(billGroupsService));
    }

    [HttpGet("{id:long}", Name = nameof(GetById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById([BindRequired, FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();

        if (user is null)
        {
            return Unauthorized();
        }

        var billGroup = await _billGroupsService.Get(user, id, cancellationToken);
        return Ok(billGroup);
    }

    [HttpGet("{userId:long}", Name = nameof(GetUserBillGroups))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserBillGroups([BindRequired, FromRoute] long userId, CancellationToken cancellationToken = default)
    {
        var billGroups = await _billGroupsService.GetByUserId(userId, cancellationToken);
        return Ok(billGroups);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateGroup([BindRequired, FromBody] CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();

        if (user is null)
        {
            return Unauthorized();
        }
        
        var id = await _billGroupsService.Create(user, createBillGroup, cancellationToken);
        return CreatedAtAction(nameof(GetById), id);
    }
}