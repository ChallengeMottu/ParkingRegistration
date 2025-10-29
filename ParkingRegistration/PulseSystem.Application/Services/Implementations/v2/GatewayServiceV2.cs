using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.ML.Services;
using PulseSystem.Application.Services.interfaces.v2;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Application.Services.Implementations.v2;

public class GatewayServiceV2 : IGatewayServiceV2
{
    private readonly IGatewayRepository _gatewayRepository;
    private readonly IParkingRepository _parkingRepository;
    private readonly IMapper _mapper;
    private readonly IGatewayPredictionService _mlService;

    public GatewayServiceV2(
        IGatewayRepository gatewayRepository,
        IParkingRepository parkingRepository,
        IMapper mapper,
        IGatewayPredictionService mlService) 
    {
        _gatewayRepository = gatewayRepository;
        _parkingRepository = parkingRepository;
        _mapper = mapper;
        _mlService = mlService;
    }

    public async Task<GatewayResponseDto> GetByIdAsync(long id)
    {
        var gateway = await _gatewayRepository.GetByIdAsync(id)
                      ?? throw new ResourceNotFoundException("Gateway não encontrado");

        return _mapper.Map<GatewayResponseDto>(gateway);
    }

    public async Task<GatewayResponseDto> AddAsync(GatewayRequestDto dto)
    {
        var gateway = _mapper.Map<Gateway>(dto);
        ValidateGateway(gateway);

        
        var parking = await _parkingRepository.GetByIdAsync(dto.ParkingId)
                      ?? throw new ResourceNotFoundException("Pátio não encontrado");

        int requiredGateways = _mlService.PredictGateways(parking.AvailableArea, parking.Capacity);
        var existingGateways = await _gatewayRepository.GetAllByParkingId(dto.ParkingId);
        int currentCount = existingGateways?.Count() ?? 0;

        if (currentCount >= requiredGateways)
            throw new InvalidOperationException(
                $"Não é possível adicionar o gateway: já existem {currentCount} gateways no pátio, " +
                $"quantidade máxima necessária para este pátio é {requiredGateways}."
            );

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

            int requiredGateways = _mlService.PredictGateways(newParking.AvailableArea, newParking.Capacity);
            var existingGateways = await _gatewayRepository.GetAllByParkingId(newParking.Id);
            int currentCount = existingGateways?.Count() ?? 0;

            if (currentCount >= requiredGateways)
                throw new InvalidOperationException(
                    $"Não é possível mover o gateway: já existem {currentCount} gateways no pátio, " +
                    $"quantidade máxima necessária para este pátio é {requiredGateways}."
                );
        }

        _mapper.Map(dto, existing);
        await _gatewayRepository.UpdateAsync(existing);

        return _mapper.Map<GatewayResponseDto>(existing);
    }

    private void ValidateGateway(Gateway gateway)
    {
        if (!gateway.IsValidMacAddress())
            throw new InvalidArgumentException("MAC Address inválido");
        if (!gateway.IsValidIp())
            throw new InvalidArgumentException("IP inválido");
    }
}
