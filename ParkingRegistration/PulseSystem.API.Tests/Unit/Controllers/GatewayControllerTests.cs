using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Configuration;
using PulseSystem.Controllers;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Controllers
{
    public class GatewayControllerTests
    {
        private readonly Mock<IGatewayService> _gatewayServiceMock;
        private readonly Mock<HateoasConfig> _hateoasMock;
        private readonly GatewayController _controller;

        public GatewayControllerTests()
        {
            _gatewayServiceMock = new Mock<IGatewayService>();
            _hateoasMock = new Mock<HateoasConfig>();

            _controller = new GatewayController(_gatewayServiceMock.Object);

            // Mock do UrlHelper
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
        public async Task GetById_ShouldReturnOk_WhenGatewayExists()
        {
            // Arrange
            var response = new GatewayResponseDto
            {
                Id = 1,
                Model = "Model X",
                Status = Domain.Enums.StatusGateway.Ativo,
                MacAddress = "AA:BB:CC:DD:EE:FF",
                LastIP = "192.168.0.1",
                ParkingId = 5
            };

            _gatewayServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenValidData()
        {
            // Arrange
            var request = new GatewayRequestDto
            {
                Model = "Model Y",
                Status = Domain.Enums.StatusGateway.Ativo,
                MacAddress = "FF:EE:DD:CC:BB:AA",
                LastIP = "192.168.0.2",
                ParkingId = 2
            };

            var created = new GatewayResponseDto
            {
                Id = 10,
                Model = request.Model,
                Status = request.Status,
                MacAddress = request.MacAddress,
                LastIP = request.LastIP,
                ParkingId = request.ParkingId
            };

            _gatewayServiceMock
                .Setup(s => s.AddAsync(It.IsAny<GatewayRequestDto>()))
                .ReturnsAsync(created);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(created);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            _gatewayServiceMock
                .Setup(s => s.RemoveAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var noContent = result as NoContentResult;
            noContent.Should().NotBeNull();
            noContent!.StatusCode.Should().Be(204);
            _gatewayServiceMock.Verify(s => s.RemoveAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task GetByMacAddress_ShouldReturnOk_WhenGatewayExists()
        {
            // Arrange
            var response = new GatewayResponseDto
            {
                Id = 3,
                Model = "Model Z",
                Status = Domain.Enums.StatusGateway.Ativo,
                MacAddress = "11:22:33:44:55:66",
                LastIP = "192.168.0.3",
                ParkingId = 7
            };

            _gatewayServiceMock
                .Setup(s => s.GetByMacAddressAsync(It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetByMacAddress("11:22:33:44:55:66");

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetByParkingId_ShouldReturnOk_WhenGatewaysExist()
        {
            // Arrange
            var list = new[]
            {
                new GatewayResponseDto { Id = 1, Model = "M1", Status = Domain.Enums.StatusGateway.Ativo, MacAddress = "AA", LastIP="192.168.1.1", ParkingId=5 },
                new GatewayResponseDto { Id = 2, Model = "M2", Status = Domain.Enums.StatusGateway.Ativo, MacAddress = "BB", LastIP="192.168.1.2", ParkingId=5 }
            };

            _gatewayServiceMock
                .Setup(s => s.GetAllByParkingId(It.IsAny<long>()))
                .ReturnsAsync(list);

            // Act
            var result = await _controller.GetByParkingId(5);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(list);
        }
    }
}
