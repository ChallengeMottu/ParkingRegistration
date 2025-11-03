namespace PulseSystem.API.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using PulseSystem.API.Tests.Integration.Configuration;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Domain.Enums;
using Xunit;


public class GatewayControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GatewayControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // Arrange
        var items = new List<GatewayResponseDto>
        {
            new GatewayResponseDto { Id = 1, Model = "Modelo X", Status = StatusGateway.Ativo, MacAddress = "AA:BB:CC:DD:EE:FF", LastIP = "192.168.0.1", ParkingId = 1 }
        };

        var expectedResponse = new PaginatedResult<GatewayResponseDto>(items, totalItems: 1, page: 1, pageSize: 10);

        _factory.GatewayServiceMock!
            .Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var response = await _client.GetAsync("/api/v1.0/Gateway");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_ShouldReturnGateway()
    {
        var gateway = new GatewayResponseDto
        {
            Id = 1,
            Model = "Modelo X",
            Status = StatusGateway.Ativo,
            MacAddress = "AA:BB:CC:DD:EE:FF",
            LastIP = "192.168.0.1",
            ParkingId = 1
        };

        _factory.GatewayServiceMock!
            .Setup(s => s.GetByIdAsync(1))
            .ReturnsAsync(gateway);

        var response = await _client.GetAsync("/api/v1.0/Gateway/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GatewayResponseDto>();
        result.Should().NotBeNull();
        result!.Model.Should().Be("Modelo X");
    }

    [Fact]
    public async Task Create_ShouldReturnCreated()
    {
        var request = new GatewayRequestDto
        {
            Model = "Modelo Y",
            Status = StatusGateway.Ativo,
            MacAddress = "11:22:33:44:55:66",
            LastIP = "192.168.0.2",
            ParkingId = 1
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

        _factory.GatewayServiceMock!
            .Setup(s => s.AddAsync(It.IsAny<GatewayRequestDto>()))
            .ReturnsAsync(created);

        var response = await _client.PostAsJsonAsync("/api/v1.0/Gateway", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<GatewayResponseDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Model.Should().Be(request.Model);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        var request = new GatewayRequestDto
        {
            Model = "Modelo Z",
            Status = StatusGateway.Inativo,
            MacAddress = "77:88:99:AA:BB:CC",
            LastIP = "192.168.0.3",
            ParkingId = 1
        };

        var updated = new GatewayResponseDto
        {
            Id = 1,
            Model = request.Model,
            Status = request.Status,
            MacAddress = request.MacAddress,
            LastIP = request.LastIP,
            ParkingId = request.ParkingId
        };

        _factory.GatewayServiceMock!
            .Setup(s => s.UpdateAsync(1, It.IsAny<GatewayRequestDto>()))
            .ReturnsAsync(updated);

        var response = await _client.PutAsJsonAsync("/api/v1.0/Gateway/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<GatewayResponseDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Model.Should().Be(request.Model);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        _factory.GatewayServiceMock!
            .Setup(s => s.RemoveAsync(1))
            .Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync("/api/v1.0/Gateway/1");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
