using AutoMapper;
using ClaimsModule.Application.Reserves.Dtos;
using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Claims;

public sealed class ReserveMappingProfile : Profile
{
    public ReserveMappingProfile()
    {
        CreateMap<ClaimReserveComponent, ReserveComponentDto>();
        CreateMap<ReserveHistory, ReserveHistoryDto>();
    }
}