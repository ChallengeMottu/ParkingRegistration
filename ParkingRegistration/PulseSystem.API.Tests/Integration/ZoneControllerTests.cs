using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using PulseSystem.API.Tests.Integration.Configuration;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using Xunit;

namespace PulseSystem.API.Tests.Integration
{
    public class ZoneControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ZoneControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var items = new List<ZoneResponseDto>
            {
                new ZoneResponseDto { Id = 1, Name = "Zona A", Width = 10, Length = 20, ParkingId = 1 }
            };

            var expectedResponse = new PaginatedResult<ZoneResponseDto>(items, totalItems: 1, page: 1, pageSize: 10);

            _factory.ZoneServiceMock!
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _client.GetAsync("/api/v1.0/Zone");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_ShouldReturnZone()
        {
            var zone = new ZoneResponseDto { Id = 1, Name = "Zona A", Width = 10, Length = 20, ParkingId = 1 };

            _factory.ZoneServiceMock!
                .Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(zone);

            var response = await _client.GetAsync("/api/v1.0/Zone/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ZoneResponseDto>();
            result.Should().NotBeNull();
            result!.Name.Should().Be("Zona A");
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            var request = new ZoneRequestDto
            {
                Name = "Zona B",
                Width = 15,
                Description = "Zona representando modelo MOTTU ESD",
                Length = 25,
                ParkingId = 1
            };

            var created = new ZoneResponseDto
            {
                Id = 2,
                Name = "Zona B",
                Width = 15,
                Description = "Zona representando modelo MOTTU ESD",
                Length = 25,
                ParkingId = 1
            };

            _factory.ZoneServiceMock!
                .Setup(s => s.AddAsync(It.IsAny<ZoneRequestDto>()))
                .ReturnsAsync(created);

            var response = await _client.PostAsJsonAsync("/api/v1.0/Zone", request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Update_ShouldReturnOk()
        {
            var request = new ZoneRequestDto
            {
                Name = "Zona Atualizada",
                Width = 12,
                Description = "Zona representando modelo MOTTU ESD",
                Length = 22,
                ParkingId = 1
            };

            var updated = new ZoneResponseDto
            {
                Id = 1,
                Name = "Zona Atualizada",
                Width = 12,
                Description = "Zona representando modelo MOTTU ESD",
                Length = 22,
                ParkingId = 1
            };

            _factory.ZoneServiceMock!
                .Setup(s => s.UpdateAsync(1, It.IsAny<ZoneRequestDto>()))
                .ReturnsAsync(updated);

            var response = await _client.PutAsJsonAsync("/api/v1.0/Zone/1", request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            _factory.ZoneServiceMock!
                .Setup(s => s.RemoveAsync(1))
                .Returns(Task.CompletedTask);

            var response = await _client.DeleteAsync("/api/v1.0/Zone/1");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
