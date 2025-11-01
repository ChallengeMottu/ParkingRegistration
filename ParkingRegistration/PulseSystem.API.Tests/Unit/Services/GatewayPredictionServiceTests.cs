
using FluentAssertions;
using PulseSystem.Application.ML.Models;
using PulseSystem.Application.ML.Services;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Services
{
    public class GatewayPredictionServiceTests : IDisposable
    {
        private readonly GatewayPredictionService _service;
        private readonly string _testModelPath;

        public GatewayPredictionServiceTests()
        {
            _service = new GatewayPredictionService();

            
            var modelDir = Path.Combine(AppContext.BaseDirectory, "test_models");
            Directory.CreateDirectory(modelDir);
            _testModelPath = Path.Combine(modelDir, "gatewayModel_test.zip");
        }

        [Fact]
        public void TrainAndSaveModel_ShouldCreateModelFile()
        {
            // Arrange
            var trainingData = new List<ParkingGatewayData>
            {
                new ParkingGatewayData { AvailableArea = 1000f, Capacity = 10, Gateways = 1 },
                new ParkingGatewayData { AvailableArea = 2000f, Capacity = 20, Gateways = 1 },
            };

            // Act
            _service.TrainAndSaveModel(trainingData, _testModelPath);

            // Assert
            File.Exists(_testModelPath).Should().BeTrue();
        }

        [Fact]
        public void LoadModel_ShouldLoadPreviouslySavedModel()
        {
            // Arrange
            _service.TrainAndSaveModel(new List<ParkingGatewayData>
            {
                new ParkingGatewayData { AvailableArea = 5000f, Capacity = 50, Gateways = 1 }
            }, _testModelPath);

            // Act
            _service.LoadModel(_testModelPath);

            // Assert
            var prediction = _service.PredictGateways(5000m, 50);
            prediction.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void PredictGateways_ShouldReturnValidPrediction()
        {
            // Arrange
            _service.TrainAndSaveModel(new List<ParkingGatewayData>
            {
                new ParkingGatewayData { AvailableArea = 10000f, Capacity = 100, Gateways = 1 }
            }, _testModelPath);

            _service.LoadModel(_testModelPath);

            // Act
            var predicted = _service.PredictGateways(12000m, 120);

            // Assert
            predicted.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void PredictGateways_ShouldThrowIfModelNotLoaded()
        {
            // Arrange
            var newService = new GatewayPredictionService();
            typeof(GatewayPredictionService)
                .GetField("_model", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(newService, null); 

            // Act
            Action act = () => newService.PredictGateways(1000m, 10);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Modelo não carregado. Chame LoadModel ou treine antes.");
        }

        public void Dispose()
        {
            
            if (File.Exists(_testModelPath))
                File.Delete(_testModelPath);
        }
    }
}
