using Microsoft.ML.Data;

namespace PulseSystem.Application.ML.Models;

public class GatewayAdjustmentData
{
    [LoadColumn(0)]
    public float AvailableArea { get; set; }

    [LoadColumn(1)]
    public float Capacity { get; set; }

    [LoadColumn(2)]
    public float IrregularityFactor { get; set; } 
    [LoadColumn(3)]
    public float DistanceBetweenZones { get; set; } 

    [LoadColumn(4)]
    public float Adjustment { get; set; } 
}