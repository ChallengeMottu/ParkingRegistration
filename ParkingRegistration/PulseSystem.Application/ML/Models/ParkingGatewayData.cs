using Microsoft.ML.Data;

namespace PulseSystem.Application.ML.Models;

public class ParkingGatewayData
{
    [LoadColumn(0)]
    public float AvailableArea { get; set; }   

    [LoadColumn(1)]
    public float Capacity { get; set; }        

    [LoadColumn(2)]
    public float Gateways { get; set; }        
}