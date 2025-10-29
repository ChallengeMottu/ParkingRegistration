using PulseSystem.Application.ML.Models;

namespace PulseSystem.Application.ML.Services;

public interface IGatewayPredictionService
{
    int PredictGateways(decimal availableArea, int capacity);
    void TrainAndSaveModel(IEnumerable<ParkingGatewayData> trainingData, string modelPath);
    void LoadModel(string modelPath);
}