using BillSplit.Api.Extensions;
using BillSplit.Contracts.Bill;
using BillSplit.Services.Abstractions.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BillSplit.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    public BillsController(IBillService billService)
    {
        _billService = billService ?? throw new ArgumentNullException(nameof(billService));
    }

    [HttpGet("{id:long}", Name = nameof(GetBills))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BillDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetBills([FromRoute, BindRequired] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroup = await _billService.GetBill(user, id, cancellationToken);
        return Ok(billGroup);
    }

    [HttpPost(Name = nameof(CreateBill))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBill([BindRequired, FromBody] CreateBillDto createBill, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var id = await _billService.CreateBill(user, createBill, cancellationToken);
        return CreatedAtAction(nameof(GetBills), new { id }, new { id });
    }
    
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBill([BindRequired, FromRoute] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        await _billService.Delete(user, id, cancellationToken);
        return NoContent();
    }
}