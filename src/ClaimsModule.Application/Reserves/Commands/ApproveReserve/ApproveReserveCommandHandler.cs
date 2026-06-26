using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ClaimsModule.Application.Reserves.Commands.ApproveReserve;

public sealed class ApproveReserveCommandHandler(
    IReserveRepository reserveRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IGlPostingJobScheduler glPostingJobScheduler,
    IAuditLogService auditLog,
    ILogger<ApproveReserveCommandHandler> logger)
    : IRequestHandler<ApproveReserveCommand, Unit>
{
    public async Task<Unit> Handle(ApproveReserveCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var reserve = await reserveRepository.GetByIdWithTransactionsAsync(request.ReserveId, ct)
            ?? throw new NotFoundException(nameof(ClaimReserveComponent), request.ReserveId);

        reserveRepository.SetOriginalRowVersion(reserve, request.RowVersion);

        // BR-R-03: self-approval guard
        var pendingTransaction = reserve.Transactions
            .FirstOrDefault(t => t.Id == request.TransactionId)
            ?? throw new NotFoundException(nameof(ReserveHistory), request.TransactionId);

        if (pendingTransaction.SubmittedByUserId == currentUser.UserId)
            throw new DomainException("Cannot approve your own reserve transaction.");

        // BR-R-01: authority level check
        var requiredLevel = ClaimReserveComponent.DetermineRequiredApprovalLevel(pendingTransaction.Amount);

        var hasAuthority = requiredLevel switch
        {
            ApprovalLevel.Supervisor => currentUser.Role is UserRoles.Supervisor or UserRoles.Manager,
            ApprovalLevel.Manager    => currentUser.Role is UserRoles.Manager,
            _                        => true
        };

        if (!hasAuthority)
            throw new DomainException(
                $"Insufficient authority to approve this transaction. Required: {requiredLevel}.");

        // BR-R-05: aggregate $10M limit
        var aggregateTotal = await reserveRepository.GetAggregateTotalByClaimAsync(reserve.ClaimId, ct);

        if (aggregateTotal + pendingTransaction.Amount > ReserveThresholds.ManagerLimit
            && !reserve.ManagerOverrideFlag)
            throw new DomainException(
                $"Approval would exceed aggregate limit of ${ReserveThresholds.ManagerLimit:N0}. Manager override required.");

        var transaction = reserve.ApproveTransaction(request.TransactionId, userId);

        await unitOfWork.SaveChangesAsync(ct);

        try
        {
            glPostingJobScheduler.EnqueueGlPosting(transaction.Id, transaction.IdempotencyKey);
        }
        catch (Exception ex)
        {
            // NOTE: Transaction is already saved as Approved in DB but GL posting job was not enqueued.
            // In production, this should be handled via Outbox pattern to guarantee atomicity.
            // For assessment scope, this is logged and accepted as a known limitation.
            logger.LogError(ex, "Failed to enqueue GL posting job for transaction {TransactionId}.", transaction.Id);
        }

        await auditLog.LogAsync(
            claimId: reserve.ClaimId,
            eventType: AuditEventTypes.ReserveApproved,
            description: $"Reserve transaction approved by {currentUser.UserId}.",
            relatedEntityId: transaction.Id,
            relatedEntityType: nameof(ReserveHistory),
            newValue: new { transaction.Amount, transaction.NewBalance });

        return Unit.Value;
    }
}