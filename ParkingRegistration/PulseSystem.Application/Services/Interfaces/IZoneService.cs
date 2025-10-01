using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;

namespace PulseSystem.Application.Services.interfaces;

public interface IZoneService
{
    Task<ZoneResponseDto> AddAsync(ZoneRequestDto dto);
    Task<ZoneResponseDto> UpdateAsync(long id, ZoneRequestDto dto);
    Task RemoveAsync(long id);
    
    Task<ZoneResponseDto> GetByIdAsync(long id);
    Task<PaginatedResult<ZoneResponseDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<IEnumerable<ZoneResponseDto>> GetByParkingIdAsync(long parkingId);
}