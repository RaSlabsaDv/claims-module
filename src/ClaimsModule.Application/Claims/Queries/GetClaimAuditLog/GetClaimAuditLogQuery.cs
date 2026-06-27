using MediatR;

namespace ClaimsModule.Application.Claims.Queries.GetClaimAuditLog;

public sealed record GetClaimAuditLogQuery
(
    Guid ClaimId,
    int Page = 1,
    int PageSize = 20) : IRequest<AuditLogResult>;