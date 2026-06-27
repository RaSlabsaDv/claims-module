using ClaimsModule.Application.Common.Interfaces;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.ListClaims;

public sealed record ListClaimsQuery(
    ClaimListFilter Filter,
    int Page,
    int PageSize) : IRequest<ListClaimsResult>;