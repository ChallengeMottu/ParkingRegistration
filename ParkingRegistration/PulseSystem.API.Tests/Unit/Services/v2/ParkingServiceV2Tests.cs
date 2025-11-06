using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.Services.Implementations.v2;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;
using Xunit;

namespace PulseSystem.API.Tests.Unit.Services.v2
{
    public class ParkingServiceV2Tests
    {
        private readonly Mock<IParkingRepository> _mockParkingRepo;
        private readonly IMapper _mapper;
        private readonly ParkingServiceV2 _service;

        public ParkingServiceV2Tests()
        {
            _mockParkingRepo = new Mock<IParkingRepository>();

            
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
            var configExpression = new MapperConfigurationExpression();
            configExpression.CreateMap<ParkingRequestDto, Parking>();
            configExpression.CreateMap<Parking, ParkingResponseDto>();
            configExpression.CreateMap<Parking, ParkingResponseListDto>();
            configExpression.CreateMap<Parking, ParkingSuggestionDto>();

            var mapperConfig = new MapperConfiguration(configExpression, loggerFactory);
            _mapper = new Mapper(mapperConfig);

            _service = new ParkingServiceV2(_mockParkingRepo.Object, _mapper);
        }

        [Fact]
        public async Task AddAsync_ShouldAddParking_WhenValid()
        {
            var dto = new ParkingRequestDto
            {
                Name = "Pátio Teste",
                AvailableArea = 1000,
                Capacity = 50
            };

            _mockParkingRepo.Setup(r => r.AddAsync(It.IsAny<Parking>())).Returns(Task.CompletedTask);

            var result = await _service.AddAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            _mockParkingRepo.Verify(r => r.AddAsync(It.IsAny<Parking>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnParking_WhenExists()
        {
            var parking = new Parking { Id = 1, Name = "Pátio 1" };
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Pátio 1");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var act = async () => await _service.GetByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateParking_WhenExists()
        {
            var existing = new Parking { Id = 1, Name = "Old Name", AvailableArea = 500, Capacity = 50 };
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockParkingRepo.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            var dto = new ParkingRequestDto { Name = "New Name", AvailableArea = 1000, Capacity = 100 };

            var result = await _service.UpdateAsync(1, dto);

            result.Name.Should().Be("New Name");
            _mockParkingRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var dto = new ParkingRequestDto { Name = "New Name", AvailableArea = 1000, Capacity = 100 };

            var act = async () => await _service.UpdateAsync(1, dto);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallRemove_WhenExists()
        {
            var parking = new Parking { Id = 1 };
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);
            _mockParkingRepo.Setup(r => r.RemoveAsync(parking)).Returns(Task.CompletedTask);

            await _service.RemoveAsync(1);

            _mockParkingRepo.Verify(r => r.RemoveAsync(parking), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var act = async () => await _service.RemoveAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task GetStructurePlanByIdAsync_ShouldReturnStructurePlan_WhenExists()
        {
            var parking = new Parking { Id = 1, StructurePlan = "Plano XYZ" };
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);

            var result = await _service.GetStructurePlanByIdAsync(1);

            result.Should().Be("Plano XYZ");
        }

        [Fact]
        public async Task GetStructurePlanByIdAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockParkingRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var act = async () => await _service.GetStructurePlanByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
