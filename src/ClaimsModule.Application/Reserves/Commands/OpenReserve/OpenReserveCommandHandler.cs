using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Reserves.Dtos;
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
    IAuditLogService auditLog) : IRequestHandler<OpenReserveCommand, OpenReserveResult>
{
    public async Task<OpenReserveResult> Handle(OpenReserveCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        claimRepository.SetOriginalRowVersion(claim, request.RowVersion);

        var reserve = ClaimReserveComponent.Create(
            claimId: request.ClaimId,
            componentType: request.ComponentType,
            createdByUserId: userId,
            notes: request.Notes);
        
        var changeSequence = await reserveRepository.GetNextChangeSequenceAsync(reserve.Id, ct);
        
        var transaction = reserve.AddTransaction(
            amount: request.Amount,
            transactionType: ReserveTransactionType.Add,
            changeReason: request.ChangeReason,
            submittedByUserId: userId,
            changeSequence: changeSequence);
        
        var approvalLevel = ClaimReserveComponent.DetermineRequiredApprovalLevel(request.Amount);
        
        if (approvalLevel == ApprovalLevel.AutoApproved)
        {
            transaction.AutoApprove();
            reserve.ApplyApprovedTransaction(transaction.Id, transaction.NewBalance);
        }
        
        await reserveRepository.AddAsync(reserve, ct);
        await unitOfWork.SaveChangesAsync(ct);
        
        if (approvalLevel == ApprovalLevel.AutoApproved)
            glPostingJobScheduler.EnqueueGlPosting(transaction.Id, transaction.IdempotencyKey);

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

        var updatedClaim = await claimRepository.GetByIdAsync(request.ClaimId, ct);
        return new OpenReserveResult(reserve.Id, Convert.ToBase64String(updatedClaim!.RowVer));
    }
}