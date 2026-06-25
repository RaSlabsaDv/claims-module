using ClaimsModule.Application.Claims.Dtos;
using MediatR;

public sealed record GetClaimDocumentsQuery(Guid ClaimId) : IRequest<IReadOnlyList<ClaimDocumentDto>>;