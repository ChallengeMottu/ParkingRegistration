using AutoMapper;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Application.configuration;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<ParkingRequestDto, Parking>();
        CreateMap<Parking, ParkingResponseDto>();
        CreateMap<Zone, ZoneSummaryDto>();
        CreateMap<Gateway, GatewaySummaryDto>();
        CreateMap<Parking, ParkingResponseListDto>();
        CreateMap<Parking, ParkingSuggestionDto>();
        
        CreateMap<GatewayRequestDto, Gateway>();
        CreateMap<Gateway, GatewayResponseDto>();
        
        CreateMap<ZoneRequestDto, Zone>();
        CreateMap<Zone, ZoneResponseDto>();
        
        
    }
}