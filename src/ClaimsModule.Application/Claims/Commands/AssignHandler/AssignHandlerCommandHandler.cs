using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.AssignHandler;

public sealed class AssignHandlerCommandHandler(
    IClaimRepository claimRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<AssignHandlerCommand, Unit>
{
    public async Task<Unit> Handle(AssignHandlerCommand request, CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        claim.AssignHandler(request.HandlerId, currentUser.UserId);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.StatusChanged,
            description: $"Handler assigned to claim.",
            relatedEntityId: request.HandlerId,
            relatedEntityType: "User",
            newValue: new { HandlerId = request.HandlerId });

        return Unit.Value;
    }
}