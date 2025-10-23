using PulseSystem.Application.DTOs.Hateoas;

namespace PulseSystem.Application.DTOs.responses;

public class ParkingSuggestionDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal AvailableArea { get; set; }
    public int Capacity { get; set; }
    public string ZoneSuggestionMessage { get; set; } 
    public int SuggestedGateways { get; set; } 
    
    public List<LinkDto> Links { get; set; } = new List<LinkDto>();
}