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
    
    [HttpGet(Name = nameof(GetAllBillGroups))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserBillGroupDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllBillGroups(CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroups = await _billGroupsService.Get(user, cancellationToken);
        return Ok(billGroups);
    }

    [HttpGet("{id:long}", Name = nameof(GetBillGroupById))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BillGroupDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetBillGroupById([FromRoute, BindRequired] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroup = await _billGroupsService.Get(user, id, cancellationToken);
        return Ok(billGroup);
    }
    
    [HttpPost(Name = nameof(CreateBillGroup))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBillGroup([BindRequired, FromBody] CreateBillGroupDto createBillGroup, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var id = await _billGroupsService.Create(user, createBillGroup, cancellationToken);
        return CreatedAtAction(nameof(GetBillGroupById), new { id }, new { id });
    }
}