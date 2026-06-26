using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.CreateClaim;

public sealed class CreateClaimCommandHandler(
    IClaimRepository claimRepository,
    IPolicyRepository policyRepository,
    ICauseOfLossCodeRepository causeOfLossCodeRepository,
    IClaimNumberGenerator claimNumberGenerator,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<CreateClaimCommand, CreateClaimResult>
{
    public async Task<CreateClaimResult> Handle(CreateClaimCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();
        var organisationId = currentUser.OrganisationId ?? throw new UnauthorizedException();

        // Валідація CauseOfLossCode
        var causeOfLossCode = await causeOfLossCodeRepository.GetByCodeAsync(request.CauseOfLossCode, ct)
            ?? throw new NotFoundException(nameof(CauseOfLossCode), request.CauseOfLossCode);

        if (!causeOfLossCode.IsActive)
            throw new DomainException($"Cause of loss code '{request.CauseOfLossCode}' is not active.");

        // Валідація Policy якщо передана
        if (request.PolicyId.HasValue)
        {
            var policy = await policyRepository.GetByIdAsync(request.PolicyId.Value, ct)
                ?? throw new NotFoundException(nameof(Policy), request.PolicyId.Value);

            if (!policy.IsActiveOn(DateOnly.FromDateTime(request.LossDate.DateTime)))
                throw new DomainException($"Policy '{policy.PolicyNumber}' is not active on loss date.");
        }

        // Атомарна генерація ClaimNumber
        var claimNumber = await claimNumberGenerator.GenerateAsync(request.OrganisationId, ct);

        // Claim агрегат створює себе і LossEvent
        var claim = Claim.Create(
            organisationId: request.OrganisationId,
            claimNumber: claimNumber,
            severity: request.Severity,
            createdByUserId: userId,
            lossDate: request.LossDate,
            lossDescription: request.LossDescription,
            causeOfLossCode: request.CauseOfLossCode,
            policyId: request.PolicyId,
            policyNumber: request.PolicyNumber,
            clientName: request.ClientName,
            assignedHandlerId: request.AssignedHandlerId,
            notes: request.Notes,
            lossLocation: request.LossLocation,
            estimatedLossAmount: request.EstimatedLossAmount,
            policeReportNumber: request.PoliceReportNumber);

        await claimRepository.AddAsync(claim, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.ClaimCreated,
            description: $"Claim {claimNumber} created.",
            newValue: new
            {
                claim.ClaimNumber,
                claim.PolicyNumber,
                claim.ClientName,
                claim.Severity,
                LossDate = request.LossDate,
                request.CauseOfLossCode
            });

        return new CreateClaimResult(claim.Id, claimNumber);
    }
}
