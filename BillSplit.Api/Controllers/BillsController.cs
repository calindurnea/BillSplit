using System.ComponentModel.DataAnnotations;
using BillSplit.Api.Extensions;
using BillSplit.Contracts.Bill;
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
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;

    /// <summary>
    /// Provides functionality for managing bills
    /// </summary>
    /// <param name="billService"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public BillsController(IBillService billService)
    {
        _billService = billService ?? throw new ArgumentNullException(nameof(billService));
    }

    /// <summary>
    /// Returns the bill with the specified id
    /// </summary>
    /// <param name="id">The id of the bill</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The bill if successful, an error otherwise</returns>
    [HttpGet("{id:long}", Name = nameof(GetBillById))]
    [ProducesResponseType(typeof(BillDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBillById([FromRoute, BindRequired] long id, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        var billDto = await _billService.GetBill(user.Result, id, cancellationToken);
        return Ok(billDto);
    }

    /// <summary>
    /// Creates or updates a bill if it already exists
    /// </summary>
    /// <param name="upsertBill">Bill to create or update</param>
    /// <param name="cancellationToken"></param>
    [HttpPut(Name = nameof(UpsertBill))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpsertBill([FromBody, Required] UpsertBillDto upsertBill, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        var id = await _billService.UpsertBill(user.Result, upsertBill, cancellationToken);
        return CreatedAtAction(nameof(GetBillById), new { id }, new { id });
    }

    /// <summary>
    /// Deletes a bill if the user is part of the group
    /// </summary>
    /// <param name="id">Id of the bill</param>
    /// <param name="cancellationToken"></param>
    /// <returns>No content if successful, an error otherwise</returns>
    [HttpDelete("{id:long}", Name = nameof(DeleteBill))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBill([FromRoute, BindRequired] long id, CancellationToken cancellationToken)
    {
        var userResult = HttpContext.User.GetCurrentUserResult();

        if (userResult is not Result.ISuccessResult<UserClaims> user)
        {
            return ResultExtensions.HandleFailedResult(userResult);
        }
        
        await _billService.Delete(user.Result, id, cancellationToken);
        return NoContent();
    }
}