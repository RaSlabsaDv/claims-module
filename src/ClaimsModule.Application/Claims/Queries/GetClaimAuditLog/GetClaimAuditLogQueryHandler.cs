using ClaimsModule.Application.Common.Interfaces;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.GetClaimAuditLog;

public sealed class GetClaimAuditLogQueryHandler
(
    IClaimAuditLogRepository claimAuditLogRepository
) : IRequestHandler<GetClaimAuditLogQuery, AuditLogResult>
{
    public async Task<AuditLogResult> Handle(GetClaimAuditLogQuery request, CancellationToken ct)
    {
        var (logs, totalCount) = await claimAuditLogRepository.ListByClaimAsync
        (
            request.ClaimId,
            request.Page,
            request.PageSize,
            ct);

        var items = logs.Select(l => new AuditLogEntryDto(
            Id: l.Id,
            EventType: l.EventType,
            Description: l.Description,
            OldValue: l.OldValue,
            NewValue: l.NewValue,
            RelatedEntityId: l.RelatedEntityId,
            RelatedEntityType: l.RelatedEntityType,
            CorrelationId: l.CorrelationId,
            CreatedAt: l.CreatedAt,
            CreatedByUserId: l.CreatedByUserId))
            .ToList();

        return new AuditLogResult(items, totalCount);
    }
}