using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Application.Exceptions;
using PulseSystem.Application.Services.interfaces;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Application.Services.Implementations
{
    public class ZoneService : IZoneService
    {
        private readonly IZoneRepository _zoneRepository;
        private readonly IParkingRepository _parkingRepository;
        private readonly IMapper _mapper;

        public ZoneService(
            IZoneRepository zoneRepository, 
            IParkingRepository parkingRepository, 
            IMapper mapper)
        {
            _zoneRepository = zoneRepository;
            _parkingRepository = parkingRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<ZoneResponseDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _zoneRepository.Query();
            var totalItems = await query.CountAsync();

            var zones = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtoList = _mapper.Map<List<ZoneResponseDto>>(zones);

            return new PaginatedResult<ZoneResponseDto>(
                items: dtoList,
                totalItems: totalItems,
                page: pageNumber,
                pageSize: pageSize
            );
        }
        
        public async Task<ZoneResponseDto> GetByIdAsync(long id)
        {
            var zone = await _zoneRepository.GetByIdAsync(id)
                       ?? throw new ResourceNotFoundException("Zona não encontrada");

            return _mapper.Map<ZoneResponseDto>(zone);
        }


        public async Task<ZoneResponseDto> AddAsync(ZoneRequestDto dto)
        {
            var parking = await _parkingRepository.GetByIdAsync(dto.ParkingId)
                          ?? throw new ResourceNotFoundException("Pátio não encontrado");

            var zone = _mapper.Map<Zone>(dto);

            if (!zone.IsValidDimensions())
                throw new InvalidArgumentException("Dimensões da zona inválidas.");

            if (parking.HasReachedMaxZones())
                throw new BusinessRuleException("Um pátio não pode ter mais de 4 zonas.");

            if (parking.ExceedsAvailableArea(zone))
                throw new BusinessRuleException("A soma das áreas das zonas ultrapassa a área disponível do pátio.");

            await _zoneRepository.AddAsync(zone);

            return _mapper.Map<ZoneResponseDto>(zone);
        }


        public async Task<ZoneResponseDto> UpdateAsync(long id, ZoneRequestDto dto)
        {
            var existing = await _zoneRepository.GetByIdAsync(id)
                           ?? throw new ResourceNotFoundException("Zona não encontrada");

            
            if (dto.ParkingId != existing.ParkingId)
            {
                if (dto.ParkingId != existing.ParkingId)
                {
                    var parking = await _parkingRepository.GetByIdAsync(dto.ParkingId)
                                  ?? throw new ResourceNotFoundException("Pátio não encontrado");

                    if (parking.HasReachedMaxZones())
                        throw new BusinessRuleException("O pátio já atingiu o limite máximo de zonas permitido.");

                    if (parking.ExceedsAvailableArea(existing))
                        throw new BusinessRuleException("A soma das áreas das zonas ultrapassa a área disponível do pátio.");
                }

            }

            
            _mapper.Map(dto, existing);

            ValidateZone(existing);

            await _zoneRepository.UpdateAsync(existing);

            return _mapper.Map<ZoneResponseDto>(existing);
        }

        public async Task RemoveAsync(long id)
        {
            var existing = await _zoneRepository.GetByIdAsync(id)
                           ?? throw new ResourceNotFoundException("Zona não encontrada");

            await _zoneRepository.RemoveAsync(existing);
        }

        public async Task<IEnumerable<ZoneResponseDto>> GetByParkingIdAsync(long parkingId)
        {
            var zones = await _zoneRepository.GetByParkingIdAsync(parkingId);

            if (zones == null || !zones.Any())
                throw new ResourceNotFoundException("Nenhuma zona encontrada para este pátio");

            return _mapper.Map<IEnumerable<ZoneResponseDto>>(zones);
        }

        private void ValidateZone(Zone zone)
        {
            if (!zone.IsValidDimensions())
                throw new InvalidArgumentException("Dimensões da zona inválidas: Width e Length devem ser maiores que 0");
        }
    }
}
