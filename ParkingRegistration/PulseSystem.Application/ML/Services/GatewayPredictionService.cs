using Microsoft.ML;
using PulseSystem.Application.ML.Models;

namespace PulseSystem.Application.ML.Services;

public class GatewayPredictionService : IGatewayPredictionService
{
    private readonly MLContext _mlContext;
    private ITransformer _model;
    private DataViewSchema _modelSchema;

    private readonly string _modelPath;

    private readonly List<ParkingGatewayData> DefaultTrainingData = new List<ParkingGatewayData>
    {
        new ParkingGatewayData { AvailableArea = 5000f, Capacity = 50, Gateways = 1 },
        new ParkingGatewayData { AvailableArea = 10000f, Capacity = 100, Gateways = 1 },
        new ParkingGatewayData { AvailableArea = 15000f, Capacity = 150, Gateways = 2 },
        new ParkingGatewayData { AvailableArea = 20000f, Capacity = 200, Gateways = 2 },
        new ParkingGatewayData { AvailableArea = 30000f, Capacity = 300, Gateways = 3 },
        new ParkingGatewayData { AvailableArea = 40000f, Capacity = 400, Gateways = 4 },
        new ParkingGatewayData { AvailableArea = 50000f, Capacity = 500, Gateways = 5 },
    };

    public GatewayPredictionService()
    {
        _mlContext = new MLContext(seed: 0);

        
        var modelDir = Path.Combine(AppContext.BaseDirectory, "models");
        Directory.CreateDirectory(modelDir);
        _modelPath = Path.Combine(modelDir, "gatewayModel.zip");

        
        InitializeModel();
    }

    private void InitializeModel()
    {
        if (File.Exists(_modelPath))
        {
            LoadModel(_modelPath);
        }
        else
        {
            TrainAndSaveModel(DefaultTrainingData, _modelPath);
            LoadModel(_modelPath);
        }
    }

    public void TrainAndSaveModel(IEnumerable<ParkingGatewayData> trainingData, string modelPath)
    {
        var data = _mlContext.Data.LoadFromEnumerable(trainingData);

        var pipeline = _mlContext.Transforms
            .Concatenate("Features", nameof(ParkingGatewayData.AvailableArea), nameof(ParkingGatewayData.Capacity))
            .Append(_mlContext.Transforms.NormalizeMeanVariance("Features")) // opcional
            .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: nameof(ParkingGatewayData.Gateways)));

        _model = pipeline.Fit(data);
        _modelSchema = data.Schema;

        _mlContext.Model.Save(_model, _modelSchema, modelPath);
    }

    public void LoadModel(string modelPath)
    {
        using var stream = File.OpenRead(modelPath);
        _model = _mlContext.Model.Load(stream, out _modelSchema);
    }

    public int PredictGateways(decimal availableArea, int capacity)
    {
        if (_model == null)
            throw new InvalidOperationException("Modelo não carregado. Chame LoadModel ou treine antes.");

        var predEngine = _mlContext.Model.CreatePredictionEngine<ParkingGatewayData, ParkingGatewayPrediction>(_model);

        var input = new ParkingGatewayData
        {
            AvailableArea = (float)availableArea,
            Capacity = (float)capacity,
        };

        var prediction = predEngine.Predict(input);
        return (int)Math.Max(0, Math.Round(prediction.PredictedGateways));
    }
}
