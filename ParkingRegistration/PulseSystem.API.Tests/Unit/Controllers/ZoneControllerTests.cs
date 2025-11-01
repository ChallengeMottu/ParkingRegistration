using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Configuration;
using PulseSystem.Controllers;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Controllers
{
    public class ZoneControllerTests
    {
        private readonly Mock<IZoneService> _zoneServiceMock;
        private readonly Mock<HateoasConfig> _hateoasMock;
        private readonly ZoneController _controller;

        public ZoneControllerTests()
        {
            _zoneServiceMock = new Mock<IZoneService>();
            _hateoasMock = new Mock<HateoasConfig>();

            // Cria controller injetando serviço mock
            _controller = new ZoneController(_zoneServiceMock.Object)
            {
                Url = new Mock<IUrlHelper>().Object
            };

            // Mock de UrlHelper para evitar NullReferenceException
            var urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock
                .Setup(u => u.Link(It.IsAny<string>(), It.IsAny<object>()))
                .Returns((string routeName, object values) => $"http://localhost/{routeName}");
            _controller.Url = urlHelperMock.Object;
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenZoneExists()
        {
            var response = new ZoneResponseDto
            {
                Id = 1,
                Name = "Zona A",
                Description = "Descrição da Zona",
                Width = 10,
                Length = 20,
                ParkingId = 1
            };

            _zoneServiceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(response);

            var result = await _controller.GetById(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetAll_ShouldReturnPaginatedResult()
        {
            var list = new PaginatedResult<ZoneResponseDto>(
                new List<ZoneResponseDto>
                {
                    new ZoneResponseDto { Id = 1, Name = "Zona A", Width = 10, Length = 20, ParkingId = 1 },
                    new ZoneResponseDto { Id = 2, Name = "Zona B", Width = 15, Length = 25, ParkingId = 1 }
                },
                totalItems: 2,
                page: 1,
                pageSize: 10
            );

            _zoneServiceMock.Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(list);

            var result = await _controller.GetAll();

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(list);
        }

        [Fact]
        public async Task Create_ShouldReturnCreated_WhenValid()
        {
            var request = new ZoneRequestDto
            {
                Name = "Zona Nova",
                Description = "Nova zona",
                Width = 12,
                Length = 18,
                ParkingId = 1
            };

            var created = new ZoneResponseDto
            {
                Id = 10,
                Name = request.Name,
                Description = request.Description,
                Width = request.Width,
                Length = request.Length,
                ParkingId = request.ParkingId
            };

            _zoneServiceMock.Setup(s => s.AddAsync(It.IsAny<ZoneRequestDto>())).ReturnsAsync(created);

            var result = await _controller.Create(request);

            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(created);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenZoneExists()
        {
            var request = new ZoneRequestDto
            {
                Name = "Zona Atualizada",
                Description = "Descrição atualizada",
                Width = 14,
                Length = 22,
                ParkingId = 1
            };

            var updated = new ZoneResponseDto
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                Width = request.Width,
                Length = request.Length,
                ParkingId = request.ParkingId
            };

            _zoneServiceMock.Setup(s => s.UpdateAsync(1, request)).ReturnsAsync(updated);

            var result = await _controller.Update(1, request);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(updated);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent_WhenDeletedSuccessfully()
        {
            _zoneServiceMock.Setup(s => s.RemoveAsync(It.IsAny<long>())).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);

            var noContent = result as NoContentResult;
            noContent.Should().NotBeNull();
            noContent!.StatusCode.Should().Be(204);

            _zoneServiceMock.Verify(s => s.RemoveAsync(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public async Task GetByParkingId_ShouldReturnOk_WhenZonesExist()
        {
            var response = new List<ZoneResponseDto>
            {
                new ZoneResponseDto { Id = 1, Name = "Zona A", Width = 10, Length = 20, ParkingId = 1 },
                new ZoneResponseDto { Id = 2, Name = "Zona B", Width = 15, Length = 25, ParkingId = 1 }
            };

            _zoneServiceMock.Setup(s => s.GetByParkingIdAsync(1)).ReturnsAsync(response);

            var result = await _controller.GetByParkingId(1);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(response);
        }
    }
}
