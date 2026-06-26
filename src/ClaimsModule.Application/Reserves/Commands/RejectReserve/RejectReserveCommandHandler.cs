using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.RejectReserve;

public sealed class RejectReserveCommandHandler(
    IReserveRepository reserveRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<RejectReserveCommand, Unit>
{
    public async Task<Unit> Handle(RejectReserveCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var reserve = await reserveRepository.GetByIdWithTransactionsAsync(request.ReserveId, ct)
            ?? throw new NotFoundException(nameof(ClaimReserveComponent), request.ReserveId);

        reserveRepository.SetOriginalRowVersion(reserve, request.RowVersion);

        reserve.RejectTransaction(request.TransactionId, userId, request.RejectionReason);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: reserve.ClaimId,
            eventType: AuditEventTypes.ReserveRejected,
            description: $"Reserve transaction rejected. Reason: {request.RejectionReason}.",
            relatedEntityId: request.TransactionId,
            relatedEntityType: nameof(ReserveHistory),
            newValue: new { request.RejectionReason });

        return Unit.Value;
    }
}