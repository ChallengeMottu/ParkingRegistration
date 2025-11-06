using AutoMapper;
using Moq;
using Xunit;
using FluentAssertions;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.Services.Implementations.v2;
using PulseSystem.Application.ML.Services;
using PulseSystem.Domain.Entities;
using PulseSystem.Domain.Enums;
using PulseSystem.Infraestructure.Repositories.interfaces;
using Microsoft.Extensions.Logging;

namespace PulseSystem.API.Tests.Unit.Services.v2
{
    public class GatewayServiceV2Tests
    {
        private readonly Mock<IGatewayRepository> _mockGatewayRepo;
        private readonly Mock<IParkingRepository> _mockParkingRepo;
        private readonly Mock<IGatewayPredictionService> _mockMlService;
        private readonly IMapper _mapper;
        private readonly GatewayServiceV2 _service;

        public GatewayServiceV2Tests()
        {
            _mockGatewayRepo = new Mock<IGatewayRepository>();
            _mockParkingRepo = new Mock<IParkingRepository>();
            _mockMlService = new Mock<IGatewayPredictionService>();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });

            var configExpression = new MapperConfigurationExpression();
            configExpression.CreateMap<GatewayRequestDto, Gateway>();
            configExpression.CreateMap<Gateway, GatewayResponseDto>();

            var mapperConfig = new MapperConfiguration(configExpression, loggerFactory);
            _mapper = new Mapper(mapperConfig);

            _service = new GatewayServiceV2(
                _mockGatewayRepo.Object,
                _mockParkingRepo.Object,
                _mapper,
                _mockMlService.Object
            );
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGateway_WhenExists()
        {
            var gateway = new Gateway { Id = 1, MacAddress = "AA:BB:CC:DD:EE:FF", Model = "X1000", Status = StatusGateway.Ativo, LastIP = "192.168.1.1", ParkingId = 1 };
            _mockGatewayRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(gateway);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Model.Should().Be("X1000");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockGatewayRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.GetByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task AddAsync_ShouldAddGateway_WhenValid()
        {
            // Arrange
            var dto = new GatewayRequestDto
            {
                Model = "Model Y",
                MacAddress = "AA:BB:CC:DD:EE:FF", 
                LastIP = "192.168.0.2",
                Status = StatusGateway.Ativo,
                ParkingId = 1
            };

            var parking = new Parking
            {
                Id = 1,
                AvailableArea = 50000, 
                Capacity = 100         
            };

            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);

            
            _mockMlService.Setup(m => m.PredictGateways(parking.AvailableArea, parking.Capacity, 0, 0)).Returns(5);

            _mockGatewayRepo.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway>());
            _mockGatewayRepo.Setup(r => r.AddAsync(It.IsAny<Gateway>())).Returns(Task.CompletedTask);

            
            var service = new GatewayServiceV2(
                _mockGatewayRepo.Object,
                _mockParkingRepo.Object,
                _mapper,
                _mockMlService.Object
            );

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().Be(dto.Model);
            result.MacAddress.Should().Be(dto.MacAddress);

            _mockGatewayRepo.Verify(r => r.AddAsync(It.IsAny<Gateway>()), Times.Once);
        }


        [Fact]
        public async Task AddAsync_ShouldThrowInvalidOperationException_WhenExceedsRequiredGateways()
        {
            var request = new GatewayRequestDto
            {
                Model = "X1000",
                Status = StatusGateway.Ativo,
                MacAddress = "AA:BB:CC:DD:EE:FF",
                LastIP = "192.168.1.1",
                ParkingId = 1
            };
            var parking = new Parking { Id = 1, Capacity = 2, AvailableArea = 1000 };

            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);
            _mockMlService.Setup(m => m.PredictGateways(parking.AvailableArea, parking.Capacity, 0, 8)).Returns(2);
            _mockGatewayRepo.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway> { new Gateway(), new Gateway() });

            var act = async () => await _service.AddAsync(request);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallRemove_WhenExists()
        {
            var gateway = new Gateway { Id = 1 };
            _mockGatewayRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(gateway);

            await _service.RemoveAsync(1);

            _mockGatewayRepo.Verify(r => r.RemoveAsync(gateway), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockGatewayRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.RemoveAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetAllByParkingId_ShouldReturnGateways_WhenExists()
        {
            var gateways = new List<Gateway>
            {
                new Gateway { Id = 1, Model = "X1", Status = StatusGateway.Ativo, MacAddress = "AA:BB:CC:DD:EE:01", LastIP = "192.168.1.2", ParkingId = 1 },
                new Gateway { Id = 2, Model = "X2", Status = StatusGateway.Ativo, MacAddress = "AA:BB:CC:DD:EE:02", LastIP = "192.168.1.3", ParkingId = 1 }
            };
            _mockGatewayRepo.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(gateways);

            var result = await _service.GetAllByParkingId(1);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllByParkingId_ShouldThrowResourceNotFoundException_WhenEmpty()
        {
            _mockGatewayRepo.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway>());

            var act = async () => await _service.GetAllByParkingId(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetByMacAddressAsync_ShouldReturnGateway_WhenExists()
        {
            var gateway = new Gateway
            {
                MacAddress = "AA:BB:CC:DD:EE:FF",
                Model = "X1000",
                Status = StatusGateway.Ativo,
                LastIP = "192.168.1.1",
                ParkingId = 1
            };
            _mockGatewayRepo.Setup(r => r.GetByMacAddressAsync("AA:BB:CC:DD:EE:FF")).ReturnsAsync(gateway);

            var result = await _service.GetByMacAddressAsync("AA:BB:CC:DD:EE:FF");

            result.Should().NotBeNull();
            result.MacAddress.Should().Be("AA:BB:CC:DD:EE:FF");
        }

        [Fact]
        public async Task GetByMacAddressAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockGatewayRepo.Setup(r => r.GetByMacAddressAsync("AA:BB:CC:DD:EE:FF")).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.GetByMacAddressAsync("AA:BB:CC:DD:EE:FF");

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
