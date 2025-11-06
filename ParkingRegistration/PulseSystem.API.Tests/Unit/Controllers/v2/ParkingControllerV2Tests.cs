using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.ML.DTOs;
using PulseSystem.Application.ML.Services;
using PulseSystem.Application.Services.interfaces.v2;
using PulseSystem.Controllers.v2;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Controllers.V2
{
    public class ParkingControllerV2Tests
    {
        private readonly Mock<IParkingServiceV2> _serviceMock;
        private readonly Mock<IGatewayPredictionService> _mlServiceMock;
        private readonly ParkingControllerV2 _controller;

        public ParkingControllerV2Tests()
        {
            _serviceMock = new Mock<IParkingServiceV2>();
            _mlServiceMock = new Mock<IGatewayPredictionService>();

            _controller = new ParkingControllerV2(_serviceMock.Object, _mlServiceMock.Object);

            // Mock do IUrlHelper para HateoasConfig
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns((string routeName, object values) => $"http://localhost/{routeName}");

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _controller.Url = urlHelperMock.Object;
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenValidData()
        {
            var request = new ParkingRequestDto { Name = "Pátio Teste", AvailableArea = 1000, Capacity = 50 };
            var response = new ParkingSuggestionDto { Id = 1, Name = "Pátio Teste" };

            _serviceMock.Setup(s => s.AddAsync(request)).ReturnsAsync(response);

            var result = await _controller.Create(request);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenParkingUpdated()
        {
            var request = new ParkingRequestDto { Name = "Pátio Atualizado", AvailableArea = 1500, Capacity = 60 };
            var response = new ParkingSuggestionDto { Id = 1, Name = "Pátio Atualizado" };

            _serviceMock.Setup(s => s.UpdateAsync(1, request)).ReturnsAsync(response);

            var result = await _controller.Update(1, request);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenParkingExists()
        {
            var response = new ParkingResponseListDto { Id = 1, Name = "Pátio 1", AvailableArea = 1000, Capacity = 50 };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(response);

            var result = await _controller.GetById(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task SuggestGateways_ShouldReturnOk_WithPredictedValue()
        {
            // Arrange
            var query = new ParkingGatewayQueryDto
            {
                IrregularityFactor = 0.1f,
                DistanceBetweenZones = 5f
            };

            var parking = new ParkingResponseListDto
            {
                Id = 1,
                AvailableArea = 1000,
                Capacity = 50
            };

            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(parking);
            _mlServiceMock.Setup(m => m.PredictGateways(1000, 50, 0.1f, 5f)).Returns(4);

            // Act
            var result = await _controller.SuggestGateways(1, query);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var response = okResult.Value as ParkingGatewaySuggestionDto;
            response.Should().NotBeNull();
    
            response!.ParkingId.Should().Be(1);
            response.Area.Should().Be(1000);
            response.Capacity.Should().Be(50);
            response.IrregularityFactor.Should().Be(0.1f);
            response.DistanceBetweenZones.Should().Be(5f);
            response.SuggestedGateways.Should().Be(4);
        }





        [Fact]
        public async Task GetStructurePlanByIdAsync_ShouldReturnContentResult_WithSvg()
        {
            string svg = "<svg>...</svg>";
            _serviceMock.Setup(s => s.GetStructurePlanByIdAsync(1)).ReturnsAsync(svg);

            var result = await _controller.GetStructurePlanByIdAsync(1);

            var contentResult = result as ContentResult;
            contentResult.Should().NotBeNull();
            contentResult!.Content.Should().Be(svg);
            contentResult.ContentType.Should().Be("image/svg+xml");
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            _serviceMock.Setup(s => s.RemoveAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);

            result.Should().BeOfType<NoContentResult>();
        }
    }
}
