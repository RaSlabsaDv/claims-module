using ClaimsModule.Application.Claims.Queries.GetClaimAuditLog;
using MediatR;

public sealed record GetClaimAuditLogQuery
(
    Guid ClaimId,
    int Page = 1,
    int PageSize = 20) : IRequest<AuditLogResult>;