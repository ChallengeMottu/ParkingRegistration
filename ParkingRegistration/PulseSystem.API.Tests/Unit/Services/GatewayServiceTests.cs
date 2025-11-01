using AutoMapper;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Services
{
    public class GatewayServiceTests
    {
        private readonly Mock<IGatewayRepository> _gatewayRepoMock;
        private readonly Mock<IParkingRepository> _parkingRepoMock;
        private readonly IMapper _mapper;
        private readonly GatewayService _service;

        public GatewayServiceTests()
        {
            _gatewayRepoMock = new Mock<IGatewayRepository>();
            _parkingRepoMock = new Mock<IParkingRepository>();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
            var configExpression = new MapperConfigurationExpression();
            configExpression.CreateMap<GatewayRequestDto, Gateway>();
            configExpression.CreateMap<Gateway, GatewayResponseDto>();

            var mapperConfig = new MapperConfiguration(configExpression, loggerFactory);
            _mapper = new Mapper(mapperConfig);

            _service = new GatewayService(_gatewayRepoMock.Object, _parkingRepoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGateway_WhenExists()
        {
            var gateway = new Gateway { Id = 1, Model = "Model X", MacAddress = "AA:BB:CC", LastIP = "192.168.0.1" };
            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(gateway);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.GetByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetByMacAddressAsync_ShouldReturnGateway_WhenExists()
        {
            var gateway = new Gateway { Id = 1, MacAddress = "AA:BB:CC" };
            _gatewayRepoMock.Setup(r => r.GetByMacAddressAsync("AA:BB:CC")).ReturnsAsync(gateway);

            var result = await _service.GetByMacAddressAsync("AA:BB:CC");

            result.Should().NotBeNull();
            result.MacAddress.Should().Be("AA:BB:CC");
        }

        [Fact]
        public async Task GetByMacAddressAsync_ShouldThrow_WhenNotFound()
        {
            _gatewayRepoMock.Setup(r => r.GetByMacAddressAsync("AA:BB:CC")).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.GetByMacAddressAsync("AA:BB:CC");

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetAllByParkingId_ShouldReturnGateways_WhenExists()
        {
            var list = new List<Gateway>
            {
                new Gateway { Id = 1, ParkingId = 1 },
                new Gateway { Id = 2, ParkingId = 1 }
            };

            _gatewayRepoMock.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(list);

            var result = await _service.GetAllByParkingId(1);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllByParkingId_ShouldThrow_WhenNotFound()
        {
            _gatewayRepoMock.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway>());

            var act = async () => await _service.GetAllByParkingId(1);

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
                Status = Domain.Enums.StatusGateway.Ativo,
                ParkingId = 1
            };

            
            var parking = new Parking
            {
                Id = 1,
                AvailableArea = 50000, // valor positivo
                Capacity = 100         // valor positivo
            };

            // Mock do repositório de parkings
            var parkingRepoMock = new Mock<IParkingRepository>();
            parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);

            // Mock do repositório de gateways
            _gatewayRepoMock.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway>());
            _gatewayRepoMock.Setup(r => r.AddAsync(It.IsAny<Gateway>())).Returns(Task.CompletedTask);

            // Criando serviço com mock de parking
            var service = new GatewayService(_gatewayRepoMock.Object, parkingRepoMock.Object, _mapper);

            // Act
            var result = await service.AddAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Model.Should().Be(dto.Model);
            result.MacAddress.Should().Be(dto.MacAddress);

            _gatewayRepoMock.Verify(r => r.AddAsync(It.IsAny<Gateway>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateGateway_WhenExists()
        {
            var existing = new Gateway { Id = 1, Model = "Old Model", ParkingId = 1 };
            var dto = new GatewayRequestDto { Model = "New Model", MacAddress = "AA:BB:CC:DD:EE:FF", LastIP = "192.168.0.1", Status = Domain.Enums.StatusGateway.Ativo, ParkingId = 1 };

            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _gatewayRepoMock.Setup(r => r.GetAllByParkingId(1)).ReturnsAsync(new List<Gateway> { existing });

            var result = await _service.UpdateAsync(1, dto);

            result.Should().NotBeNull();
            result.Model.Should().Be("New Model");
            _gatewayRepoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenGatewayNotFound()
        {
            var dto = new GatewayRequestDto { Model = "New Model", MacAddress = "AA:BB:CC:DD:EE:FF", LastIP = "192.168.0.1", Status = Domain.Enums.StatusGateway.Ativo, ParkingId = 1 };
            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.UpdateAsync(1, dto);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallRemove_WhenExists()
        {
            var gateway = new Gateway { Id = 1 };
            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(gateway);

            await _service.RemoveAsync(1);

            _gatewayRepoMock.Verify(r => r.RemoveAsync(gateway), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ShouldThrow_WhenGatewayNotFound()
        {
            _gatewayRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Gateway?)null);

            var act = async () => await _service.RemoveAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        
    }
}
