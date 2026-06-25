using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.OpenReserve;

public sealed class OpenReserveCommandHandler
(
    IClaimRepository claimRepository,
    IReserveRepository reserveRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IGlPostingJobScheduler glPostingJobScheduler,
    IAuditLogService auditLog) : IRequestHandler<OpenReserveCommand, Guid>
{
    public async Task<Guid> Handle(OpenReserveCommand request, CancellationToken ct)
    {
        var exists = await claimRepository.ExistsAsync(request.ClaimId, ct);

        if (!exists)
            throw new NotFoundException(nameof(Claim), request.ClaimId);

         var reserve = ClaimReserveComponent.Create(
            claimId: request.ClaimId,
            componentType: request.ComponentType,
            createdByUserId: currentUser.UserId,
            notes: request.Notes);

        var changeSequence = 1;
        var transaction = reserve.AddTransaction(
            amount: request.Amount,
            transactionType: ReserveTransactionType.Add,
            changeReason: request.ChangeReason,
            submittedByUserId: currentUser.UserId,
            changeSequence: changeSequence);

        await reserveRepository.AddAsync(reserve, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var approvalLevel = ClaimReserveComponent.DetermineRequiredApprovalLevel(request.Amount);

        if (approvalLevel == ApprovalLevel.AutoApproved)
        {
            transaction.AutoApprove();
            reserve.ApplyApprovedTransaction(transaction.Id, transaction.NewBalance);

            glPostingJobScheduler.EnqueueGlPosting(transaction.Id, transaction.IdempotencyKey);
        }

        await auditLog.LogAsync(
            claimId: request.ClaimId,
            eventType: AuditEventTypes.ReserveCreated,
            description: $"Reserve component {request.ComponentType} opened with amount {request.Amount}.",
            relatedEntityId: reserve.Id,
            relatedEntityType: nameof(ClaimReserveComponent),
            newValue: new { reserve.ComponentType, request.Amount, ApprovalLevel = approvalLevel });

        if (approvalLevel == ApprovalLevel.AutoApproved)
            await auditLog.LogAsync(
                claimId: request.ClaimId,
                eventType: AuditEventTypes.ReserveAutoApproved,
                description: $"Reserve component {request.ComponentType} auto-approved (amount ≤ ${ReserveThresholds.AutoApprovalLimit:N0}).",
                relatedEntityId: reserve.Id,
                relatedEntityType: nameof(ClaimReserveComponent));

        return reserve.Id;
    }
}