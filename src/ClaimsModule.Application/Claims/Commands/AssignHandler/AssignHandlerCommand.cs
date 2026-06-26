using MediatR;

namespace ClaimsModule.Application.Claims.Commands.AssignHandler;

public sealed record AssignHandlerCommand(
    Guid ClaimId,
    Guid HandlerId) : IRequest<Unit>;