using MediatR;

namespace ClaimsModule.Application.Claims.Commands.UpdateNotes;

public sealed record UpdateNotesCommand(
    Guid ClaimId,
    string Notes) : IRequest<Unit>;