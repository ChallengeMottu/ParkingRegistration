using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Application.Services.Implementations;

public class ParkingService : IParkingService
{
    private readonly IParkingRepository _parkingRepository;
    private readonly IMapper _mapper;

    public ParkingService(IParkingRepository parkingRepository, IMapper mapper)
    {
        _parkingRepository = parkingRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<ParkingResponseDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _parkingRepository.Query();
        var totalItems = await query.CountAsync();

        var parkings = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoList = _mapper.Map<List<ParkingResponseDto>>(parkings);
        return new PaginatedResult<ParkingResponseDto>(dtoList, totalItems, pageNumber, pageSize);
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

    public async Task RemoveAsync(long id)
    {
        var existing = await _parkingRepository.GetByIdAsync(id)
                       ?? throw new ResourceNotFoundException("Pátio não encontrado");

        await _parkingRepository.RemoveAsync(existing);
    }

    public async Task<ParkingResponseListDto> GetByIdAsync(long id)
    {
        var parking = await _parkingRepository.GetByIdAsync(id)
                      ?? throw new ResourceNotFoundException("Pátio não encontrado");

        return _mapper.Map<ParkingResponseListDto>(parking);
    }

    public async Task<ParkingResponseDto?> GetByLocationAsync(string street, string complement)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new InvalidArgumentException("Street");
        if (string.IsNullOrWhiteSpace(complement))
            throw new InvalidArgumentException("Complement");

        var parking = await _parkingRepository.GetByLocationAsync(street, complement)
                      ?? throw new ResourceNotFoundException("Nenhum pátio encontrado nesse endereço");

        return _mapper.Map<ParkingResponseDto>(parking);
    }

   

    private ParkingSuggestionDto MapToSuggestionDto(Parking parking)
    {
        return new ParkingSuggestionDto
        {
            Id = parking.Id,
            Name = parking.Name,
            AvailableArea = parking.AvailableArea,
            Capacity = parking.Capacity,
            ZoneSuggestionMessage = GenerateZoneSuggestionMessage(parking),
            SuggestedGateways = CalculateSuggestedGateways(parking)
        };
    }


    private string GenerateZoneSuggestionMessage(Parking parking)
    {
        const int maxZones = 4;
        var zoneArea = parking.AvailableArea / maxZones;

        return $"Podem ser adicionadas {maxZones} zonas de até {zoneArea:F2} m² cada";
    }



    private int CalculateSuggestedGateways(Parking parking)
    {
        const decimal defaultMaxCoverage = 10000m; 
        const int defaultMaxCapacity = 100;        

        return parking.CalculateRequiredGateways(defaultMaxCoverage, defaultMaxCapacity);
    }
}
