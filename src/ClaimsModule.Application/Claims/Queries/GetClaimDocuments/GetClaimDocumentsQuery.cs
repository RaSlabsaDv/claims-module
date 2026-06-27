using ClaimsModule.Application.Claims.Dtos;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.GetClaimDocuments;

public sealed record GetClaimDocumentsQuery(Guid ClaimId) : IRequest<IReadOnlyList<ClaimDocumentDto>>;