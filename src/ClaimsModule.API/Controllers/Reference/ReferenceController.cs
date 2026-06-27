using ClaimsModule.Application.Reference.Queries.GetCauseOfLossCodes;
using ClaimsModule.Application.Reference.Queries.GetClaimStatuses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers.Reference;

[ApiController]
[Route("api/reference")]
[Authorize]
public sealed class ReferenceController(ISender sender) : ControllerBase
{
    [HttpGet("cause-of-loss-codes")]
    public async Task<IActionResult> GetCauseOfLossCodes(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetCauseOfLossCodesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("claim-statuses")]
    public async Task<IActionResult> GetClaimStatuses(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetClaimStatusesQuery(), ct);
        return Ok(result);
    }
}