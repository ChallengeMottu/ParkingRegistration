using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Configuration;
using PulseSystem.Controllers;
using PulseSystem.Domain.Entities;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Controllers
{
    public class ParkingControllerTests
    {
        private readonly Mock<IParkingService> _parkingServiceMock;
        private readonly Mock<HateoasConfig> _hateoasMock;
        private readonly ParkingController _controller;

        public ParkingControllerTests()
        {
            _parkingServiceMock = new Mock<IParkingService>();
            _hateoasMock = new Mock<HateoasConfig>();

            _controller = new ParkingController(_parkingServiceMock.Object, _hateoasMock.Object);

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
        public async Task GetById_ShouldReturnOk_WhenParkingExists()
        {
            // Arrange
            var response = new ParkingResponseListDto
            {
                Id = 1,
                Name = "Pátio Central",
                Capacity = 100,
                AvailableArea = 3000,
                Location = new Address { Street = "Rua A", City = "São Paulo" }
            };

            _parkingServiceMock
                .Setup(s => s.GetByIdAsync(It.IsAny<long>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);

            _hateoasMock.Verify(h => h.AddParkingLinks(It.IsAny<ParkingResponseListDto>(), It.IsAny<IUrlHelper>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenValidData()
        {
            // Arrange
            var request = new ParkingRequestDto
            {
                Name = "Novo Pátio",
                Capacity = 50,
                AvailableArea = 1500,
                structurePlan = "<svg></svg>",
                floorPlan = "<svg></svg>",
                Location = new Address
                {
                    Street = "Rua Beta",
                    City = "Campinas",
                    Cep = "13040330",
                    State = "SP"
                }
            };

            var created = new ParkingSuggestionDto
            {
                Id = 10,
                Name = request.Name
            };

            _parkingServiceMock
                .Setup(s => s.AddAsync(It.IsAny<ParkingRequestDto>()))
                .ReturnsAsync(created);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeOfType<ParkingSuggestionDto>();
            createdResult.Value.Should().BeEquivalentTo(created);

            _hateoasMock.Verify(h => h.AddParkingLinks(It.IsAny<ParkingSuggestionDto>(), It.IsAny<IUrlHelper>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            _parkingServiceMock
                .Setup(s => s.RemoveAsync(It.IsAny<long>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var noContent = result as NoContentResult;
            noContent.Should().NotBeNull();
            noContent!.StatusCode.Should().Be(204);

            _parkingServiceMock.Verify(s => s.RemoveAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task GetByLocation_ShouldReturnOk_WhenParkingExists()
        {
            // Arrange
            var response = new ParkingResponseDto
            {
                Id = 3,
                Name = "Pátio Leste",
                Capacity = 200,
                AvailableArea = 4500,
                Location = new Address { Street = "Rua Teste", Complement = "123" }
            };

            _parkingServiceMock
                .Setup(s => s.GetByLocationAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.GetByLocation("Rua Teste", "123");

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);

            _hateoasMock.Verify(h => h.AddParkingLinks(It.IsAny<ParkingResponseDto>(), It.IsAny<IUrlHelper>()), Times.Once);
        }
    }
}
