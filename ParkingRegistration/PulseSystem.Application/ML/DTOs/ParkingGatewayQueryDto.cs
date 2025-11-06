namespace PulseSystem.Application.ML.DTOs;

public class ParkingGatewayQueryDto
{
    public decimal? AvailableArea { get; set; } 
    public int? Capacity { get; set; }
    public float IrregularityFactor { get; set; } = 0f;
    public float DistanceBetweenZones { get; set; } = 0f;
}