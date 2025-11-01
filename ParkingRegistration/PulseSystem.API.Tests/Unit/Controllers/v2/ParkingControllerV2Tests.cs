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
    public class ParkingControllerV2Tests
    {
        private readonly Mock<IParkingServiceV2> _serviceMock;
        private readonly ParkingControllerV2 _controller;

        public ParkingControllerV2Tests()
        {
            _serviceMock = new Mock<IParkingServiceV2>();
            _controller = new ParkingControllerV2(_serviceMock.Object);

            // Mock do UrlHelper para Hateoas
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
            // Arrange
            var request = new ParkingRequestDto
            {
                Name = "Parking A",
                AvailableArea = 10000,
                Capacity = 100
            };

            var created = new ParkingSuggestionDto
            {
                Id = 1,
                Name = request.Name,
                AvailableArea = request.AvailableArea,
                Capacity = request.Capacity,
                ZoneSuggestionMessage = "Podem ser adicionadas 4 zonas de até 2500,00 m² cada",
                SuggestedGateways = 2
            };

            _serviceMock
                .Setup(s => s.AddAsync(It.IsAny<ParkingRequestDto>()))
                .ReturnsAsync(created);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(created);
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            createdResult.RouteValues!["id"].Should().Be(created.Id);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenValidData()
        {
            // Arrange
            var request = new ParkingRequestDto
            {
                Name = "Parking B",
                AvailableArea = 20000,
                Capacity = 200
            };

            var updated = new ParkingSuggestionDto
            {
                Id = 2,
                Name = request.Name,
                AvailableArea = request.AvailableArea,
                Capacity = request.Capacity,
                ZoneSuggestionMessage = "Podem ser adicionadas 4 zonas de até 5000,00 m² cada",
                SuggestedGateways = 4
            };

            _serviceMock
                .Setup(s => s.UpdateAsync(2, It.IsAny<ParkingRequestDto>()))
                .ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(2, request);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(updated);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenParkingExists()
        {
            // Arrange
            var parking = new ParkingResponseListDto
            {
                Id = 3,
                Name = "Parking C",
                AvailableArea = 15000,
                Capacity = 150
            };

            _serviceMock
                .Setup(s => s.GetByIdAsync(3))
                .ReturnsAsync(parking);

            // Act
            var result = await _controller.GetById(3);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(parking);
        }
    }
}
