using ClaimsModule.Application.Claims.Commands.AddParty;
using ClaimsModule.Application.Claims.Commands.AddRiskObject;
using ClaimsModule.Application.Claims.Commands.AssignHandler;
using ClaimsModule.Application.Claims.Commands.CreateClaim;
using ClaimsModule.Application.Claims.Commands.RemoveParty;
using ClaimsModule.Application.Claims.Commands.TransitionClaimStatus;
using ClaimsModule.Application.Claims.Commands.UpdateNotes;
using ClaimsModule.Application.Claims.Commands.UploadDocument;
using ClaimsModule.Application.Claims.Queries.GetClaimAuditLog;
using ClaimsModule.Application.Claims.Queries.GetClaimDetail;
using ClaimsModule.Application.Claims.Queries.GetClaimDocuments;
using ClaimsModule.Application.Claims.Queries.ListClaims;
using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClaimsModule.API.Controllers.Claims;

[ApiController]
[Route("api/claims")]
[Authorize]
public sealed class ClaimsController(ISender sender, ICurrentUserService currentUser) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListClaims(
        [FromQuery] ClaimStatus? status,
        [FromQuery] DateTimeOffset? dateFrom,
        [FromQuery] DateTimeOffset? dateTo,
        [FromQuery] Guid? assignedHandlerId,
        [FromQuery] string? causeOfLossCode,
        [FromQuery] Guid? policyId,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ListClaimsQuery(
            new ClaimListFilter(status, dateFrom, dateTo, assignedHandlerId, causeOfLossCode, policyId, search),
            page,
            pageSize), ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetClaimDetail(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetClaimDetailQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/documents")]
    public async Task<IActionResult> GetClaimDocuments(Guid id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetClaimDocumentsQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}/audit")]
    public async Task<IActionResult> GetClaimAuditLog(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetClaimAuditLogQuery(id, page, pageSize), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateClaim(
        [FromBody] CreateClaimRequest request,
        CancellationToken ct = default)
    {
        var organisationId = currentUser.OrganisationId
            ?? throw new UnauthorizedException();

        var result = await sender.Send(new CreateClaimCommand(
            OrganisationId: organisationId,
            PolicyId: request.PolicyId,
            PolicyNumber: request.PolicyNumber,
            ClientName: request.ClientName,
            Severity: request.Severity,
            AssignedHandlerId: request.AssignedHandlerId,
            Notes: request.Notes,
            LossDate: request.LossDate,
            LossDescription: request.LossDescription,
            LossLocation: request.LossLocation,
            CauseOfLossCode: request.CauseOfLossCode,
            EstimatedLossAmount: request.EstimatedLossAmount,
            PoliceReportNumber: request.PoliceReportNumber), ct);

        return CreatedAtAction(nameof(GetClaimDetail), new { id = result.ClaimId }, result);
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> TransitionStatus(
        Guid id,
        [FromBody] TransitionClaimStatusRequest request,
        CancellationToken ct = default)
    {
        await sender.Send(new TransitionClaimStatusCommand(
            id,
            Convert.FromBase64String(request.RowVersion),
            request.TargetStatus,
            request.Reason), ct);

        return NoContent();
    }

    [HttpPut("{id:guid}/handler")]
    public async Task<IActionResult> AssignHandler(
        Guid id,
        [FromBody] AssignHandlerRequest request,
        CancellationToken ct = default)
    {
        await sender.Send(new AssignHandlerCommand(id, request.HandlerId), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/notes")]
    public async Task<IActionResult> UpdateNotes(
        Guid id,
        [FromBody] UpdateNotesRequest request,
        CancellationToken ct = default)
    {
        await sender.Send(new UpdateNotesCommand(id, request.Notes), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/parties")]
    public async Task<IActionResult> AddParty(
        Guid id,
        [FromBody] AddPartyRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new AddPartyCommand(
            ClaimId: id,
            RowVersion: Convert.FromBase64String(request.RowVersion),
            PartyRole: request.PartyRole,
            PartyType: request.PartyType,
            FirstName: request.FirstName,
            LastName: request.LastName,
            CompanyName: request.CompanyName,
            Email: request.Email,
            Phone: request.Phone,
            Notes: request.Notes), ct);
    
        return CreatedAtAction(nameof(GetClaimDetail), new { id }, new
        {
            partyId = result.PartyId,
            rowVer = result.ClaimRowVer
        });
    }

    [HttpDelete("{id:guid}/parties/{partyId:guid}")]
    public async Task<IActionResult> RemoveParty(
        Guid id,
        Guid partyId,
        [FromBody] RemovePartyRequest request,
        CancellationToken ct = default)
    {
        await sender.Send(new RemovePartyCommand(id, partyId, Convert.FromBase64String(request.RowVersion)), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/risk-objects")]
    public async Task<IActionResult> AddRiskObject(
        Guid id,
        [FromBody] AddRiskObjectRequest request,
        CancellationToken ct = default)
    {
        var riskObjectId = await sender.Send(new AddRiskObjectCommand(
            ClaimId: id,
            RowVersion: Convert.FromBase64String(request.RowVersion),
            AssetType: request.AssetType,
            AssetDescription: request.AssetDescription,
            DamageDescription: request.DamageDescription,
            IsPrimary: request.IsPrimary,
            AssetReference: request.AssetReference), ct);

        return CreatedAtAction(nameof(GetClaimDetail), new { id }, new { riskObjectId });
    }

    [HttpPost("{id:guid}/documents")]
    public async Task<IActionResult> UploadDocument(
        Guid id,
        [FromForm] UploadDocumentRequest request,
        CancellationToken ct = default)
    {
        var documentId = await sender.Send(new UploadDocumentCommand(
            id,
            Convert.FromBase64String(request.RowVersion),
            request.DocumentType,
            request.DocumentName,
            request.File.ContentType,
            request.File.Length,
            request.File.OpenReadStream()), ct);

        return CreatedAtAction(nameof(GetClaimDocuments), new { id }, new { documentId });
    }
}