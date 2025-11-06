using Microsoft.ML.Data;

namespace PulseSystem.Application.ML.Models;

public class GatewayAdjustmentPrediction
{
    [ColumnName("Score")]
    public float PredictedAdjustment { get; set; }
}