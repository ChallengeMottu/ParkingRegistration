using FluentAssertions;
using PulseSystem.Application.ML.Services;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Services
{
    public class GatewayHybridPredictionServiceTests
    {
        private readonly GatewayHybridPredictionService _service;

        public GatewayHybridPredictionServiceTests()
        {
            _service = new GatewayHybridPredictionService();
        }

        [Fact]
        public void PredictGateways_ShouldReturnAtLeastBaseValue()
        {
            // Arrange
            decimal area = 5000;
            int capacity = 30;

            // Act
            var result = _service.PredictGateways(area, capacity);

            // Assert
            result.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public void PredictGateways_ShouldIncreaseWhenIrregularityHigh()
        {
            // Arrange
            decimal area = 15000;
            int capacity = 150;

            // Act
            var baseValue = _service.PredictGateways(area, capacity, 0.0f, 10f);
            var adjustedValue = _service.PredictGateways(area, capacity, 0.8f, 120f);

            // Assert
            adjustedValue.Should().BeGreaterThanOrEqualTo(baseValue);
        }

        [Fact]
        public void PredictGateways_ShouldNotReturnNegativeValues()
        {
            // Arrange
            decimal area = 1;
            int capacity = 1;

            // Act
            var result = _service.PredictGateways(area, capacity);

            // Assert
            result.Should().BeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public void PredictGateways_ShouldRespectCapacityLimit()
        {
            // Arrange
            decimal area = 1000;
            int capacity = 100;

            // Act
            var result = _service.PredictGateways(area, capacity);

            // Assert
            result.Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public void PredictGateways_ShouldRespectAreaLimit()
        {
            // Arrange
            decimal area = 25000;
            int capacity = 20;

            // Act
            var result = _service.PredictGateways(area, capacity);

            // Assert
            result.Should().BeGreaterThanOrEqualTo(3);
        }
    }
}
