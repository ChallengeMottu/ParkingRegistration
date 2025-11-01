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
    public class ParkingServiceV2Tests
    {
        private readonly Mock<IParkingRepository> _parkingRepoMock;
        private readonly Mock<IGatewayPredictionService> _mlServiceMock;
        private readonly IMapper _mapper;
        private readonly ParkingServiceV2 _service;

        public ParkingServiceV2Tests()
        {
            _parkingRepoMock = new Mock<IParkingRepository>();
            _mlServiceMock = new Mock<IGatewayPredictionService>();

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => { });
            var configExpression = new MapperConfigurationExpression();
            configExpression.CreateMap<ParkingRequestDto, Parking>();
            configExpression.CreateMap<Parking, ParkingResponseListDto>();
            var mapperConfig = new MapperConfiguration(configExpression, loggerFactory);
            _mapper = new Mapper(mapperConfig);

            _service = new ParkingServiceV2(_parkingRepoMock.Object, _mapper, _mlServiceMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnParking_WhenExists()
        {
            var parking = new Parking { Id = 1, Name = "Parking A", AvailableArea = 1000, Capacity = 50 };
            _parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(parking);

            var result = await _service.GetByIdAsync(1);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Parking A");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            _parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var act = async () => await _service.GetByIdAsync(1);

            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage("Pátio não encontrado");
        }

        [Fact]
        public async Task AddAsync_ShouldAddParkingAndReturnSuggestion()
        {
            var dto = new ParkingRequestDto { Name = "New Parking", AvailableArea = 2000, Capacity = 100 };
            _mlServiceMock.Setup(m => m.PredictGateways(dto.AvailableArea, dto.Capacity)).Returns(2);

            Parking? addedParking = null;
            _parkingRepoMock.Setup(r => r.AddAsync(It.IsAny<Parking>()))
                .Callback<Parking>(p => addedParking = p)
                .Returns(Task.CompletedTask);

            var result = await _service.AddAsync(dto);

            addedParking.Should().NotBeNull();
            addedParking!.Name.Should().Be(dto.Name);

            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            result.SuggestedGateways.Should().Be(2);
            result.ZoneSuggestionMessage.Should().Contain("Podem ser adicionadas 4 zonas");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateParkingAndReturnSuggestion_WhenExists()
        {
            var existing = new Parking { Id = 1, Name = "Old Parking", AvailableArea = 1000, Capacity = 50 };
            var dto = new ParkingRequestDto { Name = "Updated Parking", AvailableArea = 1500, Capacity = 60 };

            _parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mlServiceMock.Setup(m => m.PredictGateways(dto.AvailableArea, dto.Capacity)).Returns(3);

            var result = await _service.UpdateAsync(1, dto);

            existing.Name.Should().Be(dto.Name);
            existing.AvailableArea.Should().Be(dto.AvailableArea);
            existing.Capacity.Should().Be(dto.Capacity);

            result.Should().NotBeNull();
            result.Name.Should().Be(dto.Name);
            result.SuggestedGateways.Should().Be(3);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenParkingNotFound()
        {
            var dto = new ParkingRequestDto { Name = "Updated Parking", AvailableArea = 1500, Capacity = 60 };
            _parkingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Parking?)null);

            var act = async () => await _service.UpdateAsync(1, dto);

            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage("Pátio não encontrado");
        }
    }
}
