using ClaimsModule.Application.Reserves.Commands.AdjustReserve;
using ClaimsModule.Application.Reserves.Commands.ApproveReserve;
using ClaimsModule.Application.Reserves.Commands.OpenReserve;
using ClaimsModule.Application.Reserves.Commands.OverrideAggregateLimit;
using ClaimsModule.Application.Reserves.Commands.RejectReserve;
using ClaimsModule.Application.Reserves.Queries.ListReserves;
using ClaimsModule.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers.Reserves;

[ApiController]
[Route("api/claims/{claimId:guid}/reserves")]
[Authorize]
public sealed class ReservesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListReserves(Guid claimId, CancellationToken ct = default)
    {
        var result = await sender.Send(new ListReservesQuery(claimId), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> OpenReserve(
        Guid claimId,
        [FromBody] OpenReserveRequest request,
        CancellationToken ct = default)
    {
        var reserveId = await sender.Send(new OpenReserveCommand(
            ClaimId: claimId,
            RowVersion: Convert.FromBase64String(request.RowVersion),
            ComponentType: request.ComponentType,
            Amount: request.Amount,
            ChangeReason: request.ChangeReason,
            Notes: request.Notes), ct);

        return CreatedAtAction(nameof(ListReserves), new { claimId }, new { reserveId });
    }

    [HttpPut("{reserveId:guid}")]
    public async Task<IActionResult> AdjustReserve(
        Guid reserveId,
        [FromBody] AdjustReserveRequest request,
        CancellationToken ct = default)
    {
        await sender.Send(new AdjustReserveCommand(
            ReserveId: reserveId,
            RowVersion: Convert.FromBase64String(request.RowVersion),
            Amount: request.Amount,
            ChangeReason: request.ChangeReason), ct);

        return NoContent();
    }

    [HttpPost("{reserveId:guid}/approve")]
    [Authorize(Roles = $"{UserRoles.Supervisor},{UserRoles.Manager}")]
    public async Task<IActionResult> ApproveReserve(
        Guid reserveId,
        [FromBody] ApproveReserveRequest request,
        CancellationToken ct = default)
    {
        await sender.Send(new ApproveReserveCommand(
            ReserveId: reserveId,
            TransactionId: request.TransactionId,
            RowVersion: Convert.FromBase64String(request.RowVersion)), ct);

        return NoContent();
    }

    [HttpPost("{reserveId:guid}/reject")]
    [Authorize(Roles = $"{UserRoles.Supervisor},{UserRoles.Manager}")]
    public async Task<IActionResult> RejectReserve(
        Guid reserveId,
        [FromBody] RejectReserveRequest request,
        CancellationToken ct = default)
    {
        await sender.Send(new RejectReserveCommand(
            ReserveId: reserveId,
            TransactionId: request.TransactionId,
            RejectionReason: request.RejectionReason,
            RowVersion: Convert.FromBase64String(request.RowVersion)), ct);

        return NoContent();
    }

    [HttpPost("{reserveId:guid}/override-limit")]
    [Authorize(Roles = UserRoles.Manager)]
    public async Task<IActionResult> OverrideAggregateLimit(
        Guid reserveId,
        CancellationToken ct = default)
    {
        await sender.Send(new OverrideAggregateLimitCommand(reserveId), ct);
        return NoContent();
    }
}