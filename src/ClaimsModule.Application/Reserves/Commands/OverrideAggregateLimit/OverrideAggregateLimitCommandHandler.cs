using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.OverrideAggregateLimit;

public sealed class OverrideAggregateLimitCommandHandler(
    IReserveRepository reserveRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<OverrideAggregateLimitCommand, Unit>
{
    public async Task<Unit> Handle(OverrideAggregateLimitCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var reserve = await reserveRepository.GetByIdAsync(request.ReserveId, ct)
            ?? throw new NotFoundException(nameof(ClaimReserveComponent), request.ReserveId);

        reserve.SetManagerOverride(userId);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: reserve.ClaimId,
            eventType: AuditEventTypes.ManagerOverrideSet,
            description: $"Manager override set for reserve {request.ReserveId}.",
            relatedEntityId: reserve.Id,
            relatedEntityType: nameof(ClaimReserveComponent));

        return Unit.Value;
    }
}