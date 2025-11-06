namespace PulseSystem.Application.DTOs.responses;

public class ParkingGatewaySuggestionDto
{
    public long ParkingId { get; set; }
    public decimal Area { get; set; }
    public int Capacity { get; set; }
    public float IrregularityFactor { get; set; }
    public float DistanceBetweenZones { get; set; }
    public int SuggestedGateways { get; set; }
}