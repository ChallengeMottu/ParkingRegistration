using AutoMapper;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.ML.Services;
using PulseSystem.Application.Services.interfaces.v2;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Application.Services.Implementations.v2;

public class ParkingServiceV2 : IParkingServiceV2
    {
        private readonly IParkingRepository _parkingRepository;
        private readonly IMapper _mapper;
        private readonly IGatewayPredictionService _mlService;

        public ParkingServiceV2(IParkingRepository parkingRepository, IMapper mapper, IGatewayPredictionService mlService)
        {
            _parkingRepository = parkingRepository;
            _mapper = mapper;
            _mlService = mlService;
        }
        
        public async Task<ParkingResponseListDto> GetByIdAsync(long id)
        {
            var parking = await _parkingRepository.GetByIdAsync(id)
                          ?? throw new ResourceNotFoundException("Pátio não encontrado");

            return _mapper.Map<ParkingResponseListDto>(parking);
        }

        public async Task<ParkingSuggestionDto> AddAsync(ParkingRequestDto parkingDto)
        {
            var parking = _mapper.Map<Parking>(parkingDto);
            await _parkingRepository.AddAsync(parking);

            return MapToSuggestionDto(parking);
        }

        public async Task<ParkingSuggestionDto> UpdateAsync(long id, ParkingRequestDto parkingDto)
        {
            var existing = await _parkingRepository.GetByIdAsync(id)
                           ?? throw new ResourceNotFoundException("Pátio não encontrado");

            _mapper.Map(parkingDto, existing);
            await _parkingRepository.UpdateAsync(existing);

            return MapToSuggestionDto(existing);
        }
        
        
        private ParkingSuggestionDto MapToSuggestionDto(Parking parking)
        {
            return new ParkingSuggestionDto()
            {
                Id = parking.Id,
                Name = parking.Name,
                AvailableArea = parking.AvailableArea,
                Capacity = parking.Capacity,
                ZoneSuggestionMessage = GenerateZoneSuggestionMessage(parking),
                SuggestedGateways = CalculateSuggestedGatewaysML(parking) 
            };
        }

        private string GenerateZoneSuggestionMessage(Parking parking)
        {
            const int maxZones = 4;
            var zoneArea = parking.AvailableArea / maxZones;
            return $"Podem ser adicionadas {maxZones} zonas de até {zoneArea:F2} m² cada";
        }
        
        private int CalculateSuggestedGatewaysML(Parking parking)
        {
            
            return _mlService.PredictGateways(parking.AvailableArea, parking.Capacity);
        }
    }