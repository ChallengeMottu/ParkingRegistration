using PulseSystem.Application.DTOs.Hateoas;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Application.DTOs.responses;

public class ParkingResponseDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public Address Location { get; set; }
    public decimal AvailableArea { get; set; }
    public int Capacity { get; set; }
    public DateTime RegisterDate { get; set; } = DateTime.Now;
    public List<LinkDto> Links { get; set; } = new List<LinkDto>();
}