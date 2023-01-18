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

    [HttpGet("{id:long}", Name = nameof(Get))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BillDto))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Get([FromRoute, BindRequired] long id, CancellationToken cancellationToken = default)
    {
        var user = HttpContext.User.GetCurrentUser();
        var billGroup = await _billService.Get(user, id, cancellationToken);
        return Ok(billGroup);
    }
}