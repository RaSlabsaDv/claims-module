using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.TransitionClaimStatus;

public sealed record TransitionClaimStatusCommand(
    Guid ClaimId,
    byte[] RowVersion,
    ClaimStatus TargetStatus,
    string? Reason) : IRequest<Unit>;