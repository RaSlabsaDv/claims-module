using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.TransitionClaimStatus;

public sealed class TransitionClaimStatusCommandHandler(
    IClaimRepository claimRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<TransitionClaimStatusCommand, Unit>
{
    public async Task<Unit> Handle(TransitionClaimStatusCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var claim = await claimRepository.GetByIdWithDetailsAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        claimRepository.SetOriginalRowVersion(claim, request.RowVersion);

        var previousStatus = claim.Status;

        claim.TransitionTo(request.TargetStatus, userId, request.Reason);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.StatusChanged,
            description: $"Claim status changed from {previousStatus} to {request.TargetStatus}.",
            oldValue: new { Status = previousStatus },
            newValue: new { Status = request.TargetStatus, request.Reason });

        return Unit.Value;
    }
}