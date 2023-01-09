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

    [HttpGet("{userId:long}", Name = nameof(GetUserBillGroups))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserBillGroups([BindRequired] long userId, CancellationToken cancellationToken = default)
    {
        var billGroups = await _billGroupsService.GetByUserId(userId, cancellationToken);
        return Ok(billGroups);
    } 
}
