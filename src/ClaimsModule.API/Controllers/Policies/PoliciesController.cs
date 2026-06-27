using ClaimsModule.Application.Policies.Queries.GetPolicyById;
using ClaimsModule.Application.Policies.Queries.SearchPolicies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers.Policies;

[ApiController]
[Route("api/policies")]
[Authorize]
public sealed class PoliciesController(ISender sender) : ControllerBase
{
    [HttpGet("search")]
    public async Task<IActionResult> SearchPolicies(
        [AsParameters] SearchPoliciesRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new SearchPoliciesQuery(
            request.SearchTerm,
            request.MaxResults), ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPolicyById(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetPolicyByIdQuery(id), ct);
        return Ok(result);
    }
}