using BillSplit.Api.Models.Requests;
using BillSplit.Domain.Interfaces;
using BillSplit.Domain.Models.BillGroup;
using BillSplit.Domain.Models.User;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BillSplit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillGroupsController : ControllerBase
{
    private readonly IBillGroupService _billGroupService;

    public BillGroupsController(IBillGroupService billGroupService)
    {
        _billGroupService = billGroupService ?? throw new ArgumentNullException(nameof(billGroupService));
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BillGroup>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        var userInfo = new UserInfo(1, "", "", "", 1);
        var billGroups = await _billGroupService.Get(userInfo, cancellationToken);
        return Ok(billGroups);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(BillGroup), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([Required, FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var userInfo = new UserInfo(1, "", "", "", 1);
        var billGroup = await _billGroupService.Get(userInfo, id, cancellationToken);

        if (billGroup is null)
        {
            return NotFound();
        }

        return Ok(billGroup);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Post([FromBody] CreateBillGroupRequest request, CancellationToken cancellationToken = default)
    {
        var userInfo = new UserInfo(1, "", "", "", 1);
        var billGroup = await _billGroupService.Create(userInfo, new CreateBillGroup(request.Name), cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = billGroup.Id }, billGroup);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Put([Required, FromRoute] long id, [FromBody] UpdateBillGroupRequest request, CancellationToken cancellationToken = default)
    {
        var userInfo = new UserInfo(1, "", "", "", 1);

        if (!await _billGroupService.CanUpdateGroup(userInfo, id, cancellationToken))
        {
            return Forbid("The specified group cannot be updated");
        }

        await _billGroupService.Update(userInfo, id, new UpdateBillGroup(request.Name), cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(NoContentResult), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete([Required, FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var userInfo = new UserInfo(1, "", "", "", 1);
        if (!await _billGroupService.CanDeleteGroup(userInfo, id, cancellationToken))
        {
            return Forbid("The specified group cannot be deleted");
        }

        await _billGroupService.Delete(userInfo, id, cancellationToken);
        return NoContent();
    }
}
