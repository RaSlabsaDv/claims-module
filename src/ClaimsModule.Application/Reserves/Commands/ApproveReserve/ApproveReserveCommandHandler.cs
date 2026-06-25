using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.ApproveReserve;

public sealed class ApproveReserveCommandHandler(
    IReserveRepository reserveRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IGlPostingJobScheduler glPostingJobScheduler,
    IAuditLogService auditLog)
    : IRequestHandler<ApproveReserveCommand, Unit>
{
    public async Task<Unit> Handle(ApproveReserveCommand request, CancellationToken ct)
    {
        var reserve = await reserveRepository.GetByIdWithTransactionsAsync(request.ReserveId, ct)
            ?? throw new NotFoundException(nameof(ClaimReserveComponent), request.ReserveId);

        var transaction = reserve.Transactions
            .FirstOrDefault(t => t.Id == request.TransactionId)
            ?? throw new NotFoundException(nameof(ReserveHistory), request.TransactionId);

        // BR-R-03: self-approval guard
        if (transaction.SubmittedByUserId == currentUser.UserId)
            throw new DomainException("Cannot approve your own reserve transaction.");

        // BR-R-01: authority level check
        var requiredLevel = ClaimReserveComponent.DetermineRequiredApprovalLevel(transaction.Amount);
        var userRole = currentUser.Role;

        var hasAuthority = requiredLevel switch
        {
            ApprovalLevel.Supervisor => userRole is "Supervisor" or "Manager",
            ApprovalLevel.Manager    => userRole is "Manager",
            _                        => true
        };

        if (!hasAuthority)
            throw new DomainException(
                $"Insufficient authority to approve this transaction. Required: {requiredLevel}.");

        // BR-R-05: aggregate $10M limit
        var aggregateTotal = await reserveRepository.GetAggregateTotalByClaimAsync(reserve.ClaimId, ct);

        if (aggregateTotal + transaction.Amount > ReserveThresholds.ManagerLimit
            && !reserve.ManagerOverrideFlag)
            throw new DomainException(
                $"Approval would exceed aggregate limit of ${ReserveThresholds.ManagerLimit:N0}. Manager override required.");

        transaction.Approve(currentUser.UserId);
        reserve.ApplyApprovedTransaction(transaction.Id, transaction.NewBalance);

        await unitOfWork.SaveChangesAsync(ct);

        glPostingJobScheduler.EnqueueGlPosting(transaction.Id, transaction.IdempotencyKey);

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