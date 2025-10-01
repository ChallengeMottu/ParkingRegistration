using Microsoft.AspNetCore.Mvc;
using PulseSystem.Application.DTOs.Hateoas;
using PulseSystem.Application.DTOs.responses;

namespace PulseSystem.Configuration;

public class HateoasConfig
{
    private List<LinkDto> BuildParkingLinks(long id, IUrlHelper urlHelper)
    {
        return new List<LinkDto>
        {
            new LinkDto(urlHelper.Link("GetParkingById", new { id }), "self", "GET"),
            new LinkDto(urlHelper.Link("UpdateParking", new { id }), "update_parking", "PUT"),
            new LinkDto(urlHelper.Link("DeleteParking", new { id }), "delete_parking", "DELETE")
        };
    }

    public void AddParkingLinks(ParkingResponseDto dto, IUrlHelper urlHelper)
    {
        dto.Links ??= new List<LinkDto>();
        dto.Links.AddRange(BuildParkingLinks(dto.Id, urlHelper));
    }

    public void AddParkingLinks(ParkingResponseListDto dto, IUrlHelper urlHelper)
    {
        dto.Links ??= new List<LinkDto>();
        dto.Links.AddRange(BuildParkingLinks(dto.Id, urlHelper));
    }

    public void AddParkingLinks(ParkingSuggestionDto dto, IUrlHelper urlHelper)
    {
        dto.Links ??= new List<LinkDto>();
        dto.Links.AddRange(BuildParkingLinks(dto.Id, urlHelper));
    }

    public void AddGatewayLinks(GatewayResponseDto dto, IUrlHelper urlHelper)
    {
        dto.Links ??= new List<LinkDto>();

        dto.Links.Add(new LinkDto(urlHelper.Link("GetGatewayById", new { id = dto.Id }), "self", "GET"));
        dto.Links.Add(new LinkDto(urlHelper.Link("UpdateGateway", new { id = dto.Id }), "update_gateway", "PUT"));
        dto.Links.Add(new LinkDto(urlHelper.Link("DeleteGateway", new { id = dto.Id }), "delete_gateway", "DELETE"));
    }

    public void AddZoneLinks(ZoneResponseDto dto, IUrlHelper urlHelper)
    {
        dto.Links ??= new List<LinkDto>();

        dto.Links.Add(new LinkDto(urlHelper.Link("GetZoneById", new { id = dto.Id }), "self", "GET"));
        dto.Links.Add(new LinkDto(urlHelper.Link("UpdateZone", new { id = dto.Id }), "update_zone", "PUT"));
        dto.Links.Add(new LinkDto(urlHelper.Link("DeleteZone", new { id = dto.Id }), "delete_zone", "DELETE"));
    }
}
