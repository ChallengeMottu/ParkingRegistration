using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Application.Services.interfaces;

public interface IParkingService
{
    
    Task<ParkingResponseListDto> GetByIdAsync(long id);
    Task<ParkingResponseDto?> GetByLocationAsync(string street, string complement);
    
    Task<PaginatedResult<ParkingResponseDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<ParkingSuggestionDto> AddAsync(ParkingRequestDto parkingDto);
    Task<ParkingSuggestionDto> UpdateAsync(long id, ParkingRequestDto parkingDto);
    Task RemoveAsync(long id);
}