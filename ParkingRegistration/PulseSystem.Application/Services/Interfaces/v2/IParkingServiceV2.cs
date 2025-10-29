using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;

namespace PulseSystem.Application.Services.interfaces.v2;

public interface IParkingServiceV2
{
    
    Task<ParkingResponseListDto> GetByIdAsync(long id);
    Task<ParkingSuggestionDto> AddAsync(ParkingRequestDto parkingDto);
    Task<ParkingSuggestionDto> UpdateAsync(long id, ParkingRequestDto parkingDto);
    


}