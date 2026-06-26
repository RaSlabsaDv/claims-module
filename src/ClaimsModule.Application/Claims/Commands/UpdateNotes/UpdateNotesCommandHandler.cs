using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.UpdateNotes;

public sealed class UpdateNotesCommandHandler(
    IClaimRepository claimRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<UpdateNotesCommand, Unit>
{
    public async Task<Unit> Handle(UpdateNotesCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();
        
        var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        var oldNotes = claim.Notes;

        claim.UpdateNotes(request.Notes, userId);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.NotesUpdated,
            description: "Claim notes updated.",
            oldValue: new { Notes = oldNotes },
            newValue: new { Notes = request.Notes });

        return Unit.Value;
    }
}