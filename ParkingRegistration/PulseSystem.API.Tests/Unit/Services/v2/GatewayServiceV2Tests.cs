using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.ML.Services;
using PulseSystem.Application.Services.Implementations.v2;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Services.V2
{
    public class GatewayServiceV2Tests
    {
        private readonly Mock<IGatewayRepository> _gatewayRepoMock;
        private readonly Mock<IParkingRepository> _parkingRepoMock;
        private readonly Mock<IGatewayPredictionService> _mlServiceMock;
        private readonly IMapper _mapper;
        private readonly GatewayServiceV2 _service;

        public GatewayServiceV2Tests()
        {
            _gatewayRepoMock = new Mock<IGatewayRepository>();
            _parkingRepoMock = new Mock<IParkingRepository>();
            _mlServiceMock = new Mock<IGatewayPredictionService>();
            
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
            var configExpression = new MapperConfigurationExpression();
            configExpression.CreateMap<GatewayRequestDto, Gateway>();
            configExpression.CreateMap<Gateway, GatewayResponseDto>();
            var mapperConfig = new MapperConfiguration(configExpression, loggerFactory);
            _mapper = new Mapper(mapperConfig);

            _service = new GatewayServiceV2(
                _gatewayRepoMock.Object, 
                _parkingRepoMock.Object, 
                _mapper, 
                _mlServiceMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGateway_WhenExists()
        {
            var gateway = new Gateway { Id = 1, MacAddress = "AA:BB:CC:DD:EE:FF" };
            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(gateway);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.MacAddress.Should().Be("AA:BB:CC:DD:EE:FF");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.GetByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage("Gateway não encontrado");
        }

        [Fact]
        public async Task AddAsync_ShouldAddGateway_WhenValidAndCapacityNotExceeded()
        {
            var dto = new GatewayRequestDto { MacAddress = "AA:BB:CC:DD:EE:FF", LastIP = "192.168.0.1", ParkingId = 1 };
            var parking = new Parking { Id = 1, AvailableArea = 1000, Capacity = 50 };

            _parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);
            _mlServiceMock.Setup(m => m.PredictGateways(parking.AvailableArea, parking.Capacity)).Returns(2);
            _gatewayRepoMock.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway> { });
            _gatewayRepoMock.Setup(r => r.AddAsync(It.IsAny<Gateway>())).Returns(Task.CompletedTask);

            var result = await _service.AddAsync(dto);

            result.Should().NotBeNull();
            result.MacAddress.Should().Be(dto.MacAddress);
            _gatewayRepoMock.Verify(r => r.AddAsync(It.IsAny<Gateway>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ShouldThrow_WhenMaxGatewaysExceeded()
        {
            var dto = new GatewayRequestDto { MacAddress = "AA:BB:CC:DD:EE:FF", LastIP = "192.168.0.1", ParkingId = 1 };
            var parking = new Parking { Id = 1, AvailableArea = 1000, Capacity = 50 };

            _parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);
            _mlServiceMock.Setup(m => m.PredictGateways(parking.AvailableArea, parking.Capacity)).Returns(1);
            _gatewayRepoMock.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway> { new Gateway() });

            var act = async () => await _service.AddAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*quantidade máxima necessária*");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateGateway_WhenValidAndParkingSame()
        {
            var existing = new Gateway { Id = 1, MacAddress = "AA:BB:CC:DD:EE:FF", ParkingId = 1 };
            var dto = new GatewayRequestDto { MacAddress = "FF:EE:DD:CC:BB:AA", LastIP = "192.168.0.2", ParkingId = 1 };

            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _gatewayRepoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, dto);

            result.MacAddress.Should().Be(dto.MacAddress);
            _gatewayRepoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenMovingToParkingWithMaxGatewaysExceeded()
        {
            var existing = new Gateway { Id = 1, MacAddress = "AA:BB:CC:DD:EE:FF", ParkingId = 1 };
            var dto = new GatewayRequestDto { MacAddress = "FF:EE:DD:CC:BB:AA", LastIP = "192.168.0.2", ParkingId = 2 };
            var newParking = new Parking { Id = 2, AvailableArea = 1000, Capacity = 50 };

            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _parkingRepoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(newParking);
            _mlServiceMock.Setup(m => m.PredictGateways(newParking.AvailableArea, newParking.Capacity)).Returns(1);
            _gatewayRepoMock.Setup(r => r.GetAllByParkingId(2)).ReturnsAsync(new List<Gateway> { new Gateway() });

            var act = async () => await _service.UpdateAsync(1, dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*quantidade máxima necessária*");
        }
    }
}
