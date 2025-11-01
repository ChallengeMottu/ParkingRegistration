using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.Services.Implementations;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;
using AutoMapper;
using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace PulseSystem.API.Tests.Unit.Services
{
    public class ZoneServiceTests
    {
        private readonly Mock<IZoneRepository> _zoneRepoMock;
        private readonly Mock<IParkingRepository> _parkingRepoMock;
        private readonly IMapper _mapper;
        private readonly ZoneService _service;

        public ZoneServiceTests()
        {
            _zoneRepoMock = new Mock<IZoneRepository>();
            _parkingRepoMock = new Mock<IParkingRepository>();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });

            var configExpression = new MapperConfigurationExpression();
            configExpression.CreateMap<ZoneRequestDto, Zone>();
            configExpression.CreateMap<Zone, ZoneResponseDto>();
            configExpression.CreateMap<Zone, ZoneSummaryDto>();

            var mapperConfig = new MapperConfiguration(configExpression, loggerFactory);
            _mapper = new Mapper(mapperConfig);

            _service = new ZoneService(_zoneRepoMock.Object, _parkingRepoMock.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnZone_WhenExists()
        {
            var zone = new Zone { Id = 1, Name = "Zona A", Width = 10, Length = 20, ParkingId = 1 };
            _zoneRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zone);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Zona A");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _zoneRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Zone?)null);

            var act = async () => await _service.GetByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task AddAsync_ShouldAddZone_WhenValid()
        {
            var dto = new ZoneRequestDto
            {
                Name = "Nova Zona",
                Width = 10,
                Length = 15,
                ParkingId = 1
            };

            // Criar um Parking real com os atributos necessários
            var parking = new Parking
            {
                Id = 1,
                Name = "Pátio Teste",
                Capacity = 100,
                AvailableArea = 50000,
                Zones = new List<Zone>()
            };

            _parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);
            _zoneRepoMock.Setup(r => r.AddAsync(It.IsAny<Zone>())).Returns(Task.CompletedTask);

            var result = await _service.AddAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            _zoneRepoMock.Verify(r => r.AddAsync(It.IsAny<Zone>()), Times.Once);
        }


        [Fact]
        public async Task UpdateAsync_ShouldUpdateZone_WhenValid()
        {
            var existing = new Zone { Id = 1, Name = "Zona A", Width = 10, Length = 20, ParkingId = 1 };
            var dto = new ZoneRequestDto { Name = "Zona Atualizada", Width = 15, Length = 25, ParkingId = 1 };

            _zoneRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _zoneRepoMock.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(1, dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Zona Atualizada");
            _zoneRepoMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallRemove_WhenExists()
        {
            var zone = new Zone { Id = 1 };
            _zoneRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(zone);
            _zoneRepoMock.Setup(r => r.RemoveAsync(zone)).Returns(Task.CompletedTask);

            await _service.RemoveAsync(1);

            _zoneRepoMock.Verify(r => r.RemoveAsync(zone), Times.Once);
        }

        [Fact]
        public async Task GetByParkingIdAsync_ShouldReturnZones()
        {
            var list = new List<Zone>
            {
                new Zone { Id = 1, Name = "Zona A", Width = 10, Length = 20, ParkingId = 1 },
                new Zone { Id = 2, Name = "Zona B", Width = 15, Length = 25, ParkingId = 1 }
            };

            _zoneRepoMock.Setup(r => r.GetByParkingIdAsync(1)).ReturnsAsync(list);

            var result = await _service.GetByParkingIdAsync(1);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }
    }
}
