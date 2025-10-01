using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Application.Services.Implementations;

public class GatewayService : IGatewayService
{
    private readonly IGatewayRepository _gatewayRepository;
    private readonly IParkingRepository _parkingRepository;
    private readonly IMapper _mapper;

    public GatewayService(IGatewayRepository gatewayRepository, IParkingRepository parkingRepository, IMapper mapper)
    {
        _gatewayRepository = gatewayRepository;
        _parkingRepository = parkingRepository;
        _mapper = mapper;
    }


    public async Task<PaginatedResult<GatewayResponseDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _gatewayRepository.Query(); 
        var totalItems = await query.CountAsync(); 

        var gateways = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtoList = _mapper.Map<List<GatewayResponseDto>>(gateways);

        return new PaginatedResult<GatewayResponseDto>(
            items: dtoList,
            totalItems: totalItems,
            page: pageNumber,
            pageSize: pageSize
        );
    }

    public async Task<GatewayResponseDto> GetByIdAsync(long id)
    {
        var gateway = await _gatewayRepository.GetByIdAsync(id)
                      ?? throw new ResourceNotFoundException("Gateway não encontrado");

        return _mapper.Map<GatewayResponseDto>(gateway);
    }

    public async Task<GatewayResponseDto> GetByMacAddressAsync(string macAddress)
    {
        var gateway = await _gatewayRepository.GetByMacAddressAsync(macAddress)
                      ?? throw new ResourceNotFoundException($"Gateway com MAC Address {macAddress}");

        return _mapper.Map<GatewayResponseDto>(gateway);
    }

    public async Task<IEnumerable<GatewayResponseDto>> GetAllByParkingId(long parkingId)
    {
        var gateways = await _gatewayRepository.GetAllByParkingId(parkingId);
        if (gateways == null || !gateways.Any())
            throw new ResourceNotFoundException("Gateways não encontrados no pátio");

        return _mapper.Map<IEnumerable<GatewayResponseDto>>(gateways);
    }

    public async Task<GatewayResponseDto> AddAsync(GatewayRequestDto dto)
    {
        var gateway = _mapper.Map<Gateway>(dto);
        ValidateGateway(gateway);

        
        await ValidateParkingForGatewayAsync(gateway.ParkingId, gateway.MaxCoverageArea, gateway.MaxCapacity);

        await _gatewayRepository.AddAsync(gateway);
        return _mapper.Map<GatewayResponseDto>(gateway);
    }


    public async Task<GatewayResponseDto> UpdateAsync(long id, GatewayRequestDto dto)
    {
        var existing = await _gatewayRepository.GetByIdAsync(id)
                       ?? throw new ResourceNotFoundException("Gateway não encontrado");

        
        var tempGateway = _mapper.Map<Gateway>(dto); 
        ValidateGateway(tempGateway);

        
        if (dto.ParkingId != existing.ParkingId)
        {
            var newParking = await _parkingRepository.GetByIdAsync(dto.ParkingId)
                             ?? throw new ResourceNotFoundException("Pátio não encontrado");

            
            int requiredGateways = newParking.CalculateRequiredGateways(tempGateway.MaxCoverageArea, tempGateway.MaxCapacity);
            var existingGateways = await _gatewayRepository.GetAllByParkingId(newParking.Id);
            int currentCount = existingGateways?.Count() ?? 0;

            if (currentCount >= requiredGateways)
            {
                throw new InvalidOperationException(
                    $"Não é possível mover o gateway: já existem {currentCount} gateways no pátio, " +
                    $"quantidade máxima necessária para este pátio é {requiredGateways}."
                );
            }
        }

        
        _mapper.Map(dto, existing);

        await _gatewayRepository.UpdateAsync(existing);
        return _mapper.Map<GatewayResponseDto>(existing);
    }



    public async Task RemoveAsync(long id)
    {
        var gateway = await _gatewayRepository.GetByIdAsync(id)
                      ?? throw new ResourceNotFoundException("Gateway não encontrado");

        await _gatewayRepository.RemoveAsync(gateway);
    }

    private void ValidateGateway(Gateway gateway)
    {
        if (!gateway.IsValidMacAddress())
            throw new InvalidArgumentException("MAC Address");

        if (!gateway.IsValidIp())
            throw new InvalidArgumentException("IP Address");
    }
    
    private async Task<Parking> ValidateParkingForGatewayAsync(long parkingId, decimal maxCoverageArea, int maxCapacity)
    {
        var parking = await _parkingRepository.GetByIdAsync(parkingId)
                      ?? throw new ResourceNotFoundException("Pátio não encontrado");

        int requiredGateways = parking.CalculateRequiredGateways(maxCoverageArea, maxCapacity);
        var existingGateways = await _gatewayRepository.GetAllByParkingId(parkingId);
        int currentCount = existingGateways?.Count() ?? 0;

        if (currentCount >= requiredGateways)
            throw new InvalidOperationException(
                $"Não é possível adicionar ou mover o gateway: já existem {currentCount} gateways no pátio, " +
                $"quantidade máxima necessária para este pátio é {requiredGateways}."
            );

        return parking;
    }

}
