using PulseSystem.Application.DTOs.Hateoas;

namespace PulseSystem.Application.DTOs.responses;

public class ZoneResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    
    public long ParkingId { get; set; }
    public List<LinkDto> Links { get; set; } = new List<LinkDto>();
}