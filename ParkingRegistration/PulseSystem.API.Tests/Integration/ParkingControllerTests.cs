using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using PulseSystem.API.Tests.Integration.Configuration;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Domain.Entities;
using Xunit;

namespace PulseSystem.API.Tests.Integration
{
    public class ParkingControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ParkingControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var items = new List<ParkingResponseDto>
            {
                new ParkingResponseDto { Id = 1, Name = "Pátio Central" }
            };

            var expectedResponse = new PaginatedResult<ParkingResponseDto>(
                items,
                totalItems: 1,
                page: 1,
                pageSize: 10
            );

            _factory.ParkingServiceMock!
                .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _client.GetAsync("/api/v1.0/Parking");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_ShouldReturnParking()
        {
            // Arrange
            var parking = new ParkingResponseListDto { Id = 1, Name = "Teste Pátio" };

            _factory.ParkingServiceMock!
                .Setup(s => s.GetByIdAsync(1))
                .ReturnsAsync(parking);

            // Act
            var response = await _client.GetAsync("/api/v1.0/Parking/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ParkingResponseListDto>();
            result.Should().NotBeNull();
            result!.Name.Should().Be("Teste Pátio");
        }

        [Fact]
        public async Task Create_ShouldReturnCreated()
        {
            // Arrange
            var request = new ParkingRequestDto
            {
                Name = "Novo Pátio",
                Location = new Address
                {
                    Street = "Av. Paulista",
                    City = "São Paulo",
                    State = "SP",
                    Neighborhood = "Jardins",
                    Complement = "Complexo 12",
                    Cep = "01310-100"
                },
                AvailableArea = 500.0m,
                Capacity = 50,
                StructurePlan = "<svg>...</svg>",
                FloorPlan = "<svg>...</svg>",
                MapPlan = "<svg>...</svg>" 
            };

            var created = new ParkingSuggestionDto
            {
                Id = 10,
                Name = request.Name
            };

            _factory.ParkingServiceMock!
                .Setup(s => s.AddAsync(It.IsAny<ParkingRequestDto>()))
                .ReturnsAsync(created);

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1.0/Parking", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = await response.Content.ReadFromJsonAsync<ParkingSuggestionDto>();
            result.Should().NotBeNull();
            result!.Id.Should().Be(10);
            result.Name.Should().Be(request.Name);
        }


        [Fact]
        public async Task Update_ShouldReturnOk()
        {
            // Arrange
            var request = new ParkingRequestDto
            {
                Name = "Pátio Atualizado",
                Location = new Address
                {
                    Street = "Av. Paulista",
                    City = "São Paulo",
                    State = "SP",
                    Neighborhood = "Jardins",
                    Complement = "Sem complemento",
                    Cep = "01310-100"
                },
                AvailableArea = 600.0m,
                Capacity = 60,
                StructurePlan = "<svg>...</svg>",
                FloorPlan = "<svg>...</svg>",
                MapPlan = "<svg>...</svg>" 
            };

            var updated = new ParkingSuggestionDto
            {
                Id = 1,
                Name = request.Name
            };

            _factory.ParkingServiceMock!
                .Setup(s => s.UpdateAsync(1, It.IsAny<ParkingRequestDto>()))
                .ReturnsAsync(updated);

            // Act
            var response = await _client.PutAsJsonAsync("/api/v1.0/Parking/1", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ParkingSuggestionDto>();
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be(request.Name);
        }


        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            // Arrange
            _factory.ParkingServiceMock!
                .Setup(s => s.RemoveAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var response = await _client.DeleteAsync("/api/v1.0/Parking/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
