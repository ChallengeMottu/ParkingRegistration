using PulseSystem.Application.ML.Models;

namespace PulseSystem.Application.ML.Services;

public interface IGatewayPredictionService
{
    int PredictGateways(decimal availableArea, int capacity,
        float irregularityFactor = 0f, float distanceBetweenZones = 0f);

    
    void TrainAndSaveModel(IEnumerable<GatewayAdjustmentData> trainingData, string modelPath);
    void LoadModel(string modelPath);
}