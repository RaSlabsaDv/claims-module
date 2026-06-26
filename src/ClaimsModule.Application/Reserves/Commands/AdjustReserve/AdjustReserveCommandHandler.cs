using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.AdjustReserve;

public sealed class AdjustReserveCommandHandler(
    IReserveRepository reserveRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IGlPostingJobScheduler glPostingJobScheduler,
    IAuditLogService auditLog)
    : IRequestHandler<AdjustReserveCommand, Unit>
{
    public async Task<Unit> Handle(AdjustReserveCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var reserve = await reserveRepository.GetByIdWithTransactionsAsync(request.ReserveId, ct)
            ?? throw new NotFoundException(nameof(ClaimReserveComponent), request.ReserveId);

        var changeSequence = await reserveRepository.GetNextChangeSequenceAsync(request.ReserveId, ct);

        var transaction = reserve.AddTransaction(
            amount: request.Amount,
            transactionType: ReserveTransactionType.Adjust,
            changeReason: request.ChangeReason,
            submittedByUserId: userId,
            changeSequence: changeSequence);

        var approvalLevel = ClaimReserveComponent.DetermineRequiredApprovalLevel(request.Amount);

        if (approvalLevel == ApprovalLevel.AutoApproved)
        {
            transaction.AutoApprove();
            reserve.ApplyApprovedTransaction(transaction.Id, transaction.NewBalance);
        }

        await unitOfWork.SaveChangesAsync(ct);

        if (approvalLevel == ApprovalLevel.AutoApproved)
            glPostingJobScheduler.EnqueueGlPosting(transaction.Id, transaction.IdempotencyKey);

        await auditLog.LogAsync(
            claimId: reserve.ClaimId,
            eventType: AuditEventTypes.ReserveAdjusted,
            description: $"Reserve component adjusted by {request.Amount}.",
            relatedEntityId: reserve.Id,
            relatedEntityType: nameof(ClaimReserveComponent),
            newValue: new { request.Amount, request.ChangeReason, ApprovalLevel = approvalLevel });

        return Unit.Value;
    }
}