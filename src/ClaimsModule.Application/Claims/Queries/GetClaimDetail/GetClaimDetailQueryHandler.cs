using AutoMapper;
using ClaimsModule.Application.Claims.Dtos;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.GetClaimDetail;

public sealed class GetClaimDetailQueryHandler
(
    IClaimRepository claimRepository,
    IMapper mapper
) : IRequestHandler<GetClaimDetailQuery, ClaimDetailDto>
{
    public async Task<ClaimDetailDto> Handle(GetClaimDetailQuery request, CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdWithDetailsAsync(request.id, ct);

        if (claim is null)
            throw new NotFoundException(nameof(Claim), request.id);
        
        var dto = mapper.Map<ClaimDetailDto>(claim);

        return dto;
    }
}