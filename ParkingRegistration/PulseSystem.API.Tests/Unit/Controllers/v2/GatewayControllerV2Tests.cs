using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.interfaces.v2;
using PulseSystem.Controllers.v2;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Controllers.V2
{
    public class GatewayControllerV2Tests
    {
        private readonly Mock<IGatewayServiceV2> _serviceMock;
        private readonly GatewayControllerV2 _controller;

        public GatewayControllerV2Tests()
        {
            _serviceMock = new Mock<IGatewayServiceV2>();
            _controller = new GatewayControllerV2(_serviceMock.Object);
            
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

            _serviceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
            ((GatewayResponseDto)okResult.Value).Links.Should().NotBeNull(); // valida HATEOAS
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

            _serviceMock
                .Setup(s => s.AddAsync(It.IsAny<GatewayRequestDto>()))
                .ReturnsAsync(created);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(created);
            ((GatewayResponseDto)createdResult.Value).Links.Should().NotBeNull(); // valida HATEOAS
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues!["id"].Should().Be(created.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var request = new GatewayRequestDto
            {
                Model = "Model Z",
                Status = Domain.Enums.StatusGateway.Ativo,
                MacAddress = "11:22:33:44:55:66",
                LastIP = "192.168.0.3",
                ParkingId = 3
            };

            var updated = new GatewayResponseDto
            {
                Id = 5,
                Model = request.Model,
                Status = request.Status,
                MacAddress = request.MacAddress,
                LastIP = request.LastIP,
                ParkingId = request.ParkingId
            };

            _serviceMock
                .Setup(s => s.UpdateAsync(5, request))
                .ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(5, request);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(updated);
            ((GatewayResponseDto)okResult.Value).Links.Should().NotBeNull(); // valida HATEOAS
        }
    }
}
