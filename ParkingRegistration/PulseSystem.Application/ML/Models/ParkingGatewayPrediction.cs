using Microsoft.ML.Data;

namespace PulseSystem.Application.ML.Models;

public class ParkingGatewayPrediction
{
    [ColumnName("Score")]
    public float PredictedGateways { get; set; }
}