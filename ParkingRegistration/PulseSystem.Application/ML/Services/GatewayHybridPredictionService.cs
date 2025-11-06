using Microsoft.ML;
using PulseSystem.Application.ML.Models;

namespace PulseSystem.Application.ML.Services;

public class GatewayHybridPredictionService : IGatewayPredictionService
{
    private readonly MLContext _mlContext;
    private ITransformer? _model;
    private DataViewSchema? _schema;
    private readonly string _modelPath;

    
    private const float MaxCoverageAreaPerGateway = 10000f;      
    private const int MaxCapacityPerGateway = 50;               

    
    private readonly List<GatewayAdjustmentData> DefaultTrainingData = new()
    {
        new() { AvailableArea = 5000,  Capacity = 50,  IrregularityFactor = 0.0f, DistanceBetweenZones = 20,  Adjustment = 0 }, 
        new() { AvailableArea = 10000, Capacity = 100, IrregularityFactor = 0.0f, DistanceBetweenZones = 20,  Adjustment = 0 },

        new() { AvailableArea = 15000, Capacity = 150, IrregularityFactor = 0.1f, DistanceBetweenZones = 20,  Adjustment = 0 },
        new() { AvailableArea = 15000, Capacity = 150, IrregularityFactor = 0.4f, DistanceBetweenZones = 100, Adjustment = 1 },

        new() { AvailableArea = 30000, Capacity = 250, IrregularityFactor = 0.0f, DistanceBetweenZones = 30,  Adjustment = 0 },
        new() { AvailableArea = 30000, Capacity = 250, IrregularityFactor = 0.6f, DistanceBetweenZones = 120, Adjustment = 2 },

        new() { AvailableArea = 60000, Capacity = 300, IrregularityFactor = 0.0f, DistanceBetweenZones = 50,  Adjustment = 1 },
        new() { AvailableArea = 60000, Capacity = 300, IrregularityFactor = 0.8f, DistanceBetweenZones = 10,  Adjustment = 3 }, // cenário realista
    };

    public GatewayHybridPredictionService()
    {
        _mlContext = new MLContext(seed: 7);
        var modelDir = Path.Combine(AppContext.BaseDirectory, "models");
        Directory.CreateDirectory(modelDir);
        _modelPath = Path.Combine(modelDir, "gatewayHybridModel.zip");

        if (!File.Exists(_modelPath))
            TrainAndSaveModel(DefaultTrainingData, _modelPath);

        LoadModel(_modelPath);
    }

    public void TrainAndSaveModel(IEnumerable<GatewayAdjustmentData> trainingData, string modelPath)
    {
        var data = _mlContext.Data.LoadFromEnumerable(trainingData);

        var pipeline = _mlContext.Transforms
            .Concatenate("Features",
                nameof(GatewayAdjustmentData.AvailableArea),
                nameof(GatewayAdjustmentData.Capacity),
                nameof(GatewayAdjustmentData.IrregularityFactor),
                nameof(GatewayAdjustmentData.DistanceBetweenZones))
            .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(_mlContext.Regression.Trainers.Sdca(
                labelColumnName: nameof(GatewayAdjustmentData.Adjustment)
            ));

        _model = pipeline.Fit(data);
        _schema = data.Schema;

        _mlContext.Model.Save(_model, _schema, modelPath);
    }

    public void LoadModel(string modelPath)
    {
        using var fs = File.OpenRead(modelPath);
        _model = _mlContext.Model.Load(fs, out _schema);
    }


   
    public int PredictGateways(decimal availableArea, int capacity, float irregularityFactor = 0f, float distanceBetweenZones = 0f)
    {
        
        var baseByArea = (int)Math.Ceiling((double)availableArea / MaxCoverageAreaPerGateway);
        var baseByCapacity = (int)Math.Ceiling((double)capacity / MaxCapacityPerGateway);

        int baseGateways = Math.Max(baseByArea, baseByCapacity);

        
        if (_model == null)
            return baseGateways;

        
        var input = new GatewayAdjustmentData
        {
            AvailableArea = (float)availableArea,
            Capacity = capacity,
            IrregularityFactor = irregularityFactor,
            DistanceBetweenZones = distanceBetweenZones
        };

        var engine = _mlContext.Model.CreatePredictionEngine<GatewayAdjustmentData, GatewayAdjustmentPrediction>(_model);
        var pred = engine.Predict(input);

        
        int adjustment = Math.Max(0, (int)Math.Round(pred.PredictedAdjustment));

        
        return baseGateways + adjustment;
    }


    
    void IGatewayPredictionService.LoadModel(string modelPath) => LoadModel(modelPath);
    void IGatewayPredictionService.TrainAndSaveModel(IEnumerable<GatewayAdjustmentData> data, string path) =>
        TrainAndSaveModel(data, path);
}
