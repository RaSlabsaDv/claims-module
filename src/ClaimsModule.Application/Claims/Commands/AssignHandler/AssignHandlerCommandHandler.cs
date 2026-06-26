using ClaimsModule.Application.Common.Exceptions;
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
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);
    
        var previousHandlerId = claim.AssignedHandlerId;
    
        claim.AssignHandler(request.HandlerId, userId);
    
        await unitOfWork.SaveChangesAsync(ct);
    
        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.HandlerAssigned,
            description: $"Handler changed from {previousHandlerId} to {request.HandlerId}.",
            oldValue: new { HandlerId = previousHandlerId },
            newValue: new { HandlerId = request.HandlerId });
    
        return Unit.Value;
    }
}