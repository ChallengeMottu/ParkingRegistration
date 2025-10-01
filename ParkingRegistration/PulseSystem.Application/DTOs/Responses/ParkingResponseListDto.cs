using PulseSystem.Application.DTOs.Hateoas;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Application.DTOs.responses;

public class ParkingResponseListDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public Address Location { get; set; }
    public decimal AvailableArea { get; set; }
    public int Capacity { get; set; }
    public DateTime RegisterDate { get; set; }
    public List<ZoneSummaryDto> Zones { get; set; } = new();
    public List<GatewaySummaryDto> Gateways { get; set; } = new();
    public List<LinkDto> Links { get; set; } = new List<LinkDto>();
}