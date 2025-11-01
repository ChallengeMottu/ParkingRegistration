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
    public class ParkingServiceTests
    {
        private readonly Mock<IParkingRepository> _mockRepository;
        private readonly IMapper _mapper;
        private readonly ParkingService _service;

        public ParkingServiceTests()
        {
            _mockRepository = new Mock<IParkingRepository>();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
            
            var configExpression = new MapperConfigurationExpression();
            configExpression.CreateMap<ParkingRequestDto, Parking>();
            configExpression.CreateMap<Parking, ParkingResponseDto>();
            configExpression.CreateMap<Parking, ParkingResponseListDto>();
            configExpression.CreateMap<Parking, ParkingSuggestionDto>();

            var mapperConfig = new MapperConfiguration(configExpression, loggerFactory);
            _mapper = new Mapper(mapperConfig);

            _service = new ParkingService(_mockRepository.Object, _mapper);
        }

        [Fact]
        public async Task AddAsync_ShouldReturnSuggestionDto()
        {
            var request = new ParkingRequestDto
            {
                Name = "Pátio Teste",
                Capacity = 50,
                AvailableArea = 1000
            };

            var result = await _service.AddAsync(request);

            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Parking>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnParking_WhenExists()
        {
            var parking = new Parking { Id = 1, Name = "Teste" };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Name.Should().Be("Teste");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var act = async () => await _service.GetByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }
        

        [Fact]
        public async Task UpdateAsync_ShouldUpdateParking_WhenExists()
        {
            var existing = new Parking { Id = 1, Name = "Old Name" };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

            var request = new ParkingRequestDto { Name = "New Name", Capacity = 100, AvailableArea = 2000 };

            var result = await _service.UpdateAsync(1, request);

            result.Name.Should().Be("New Name");
            _mockRepository.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);
            var request = new ParkingRequestDto { Name = "New Name" };

            var act = async () => await _service.UpdateAsync(1, request);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Fact]
        public async Task RemoveAsync_ShouldCallRemove_WhenExists()
        {
            var parking = new Parking { Id = 1 };
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);

            await _service.RemoveAsync(1);

            _mockRepository.Verify(r => r.RemoveAsync(parking), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ShouldThrowResourceNotFoundException_WhenNotExists()
        {
            _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var act = async () => await _service.RemoveAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
