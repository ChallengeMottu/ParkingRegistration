using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;

public interface IGatewayService
{
    Task<PaginatedResult<GatewayResponseDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<GatewayResponseDto> GetByIdAsync(long id);
    Task<GatewayResponseDto> GetByMacAddressAsync(string macAddress);
    Task<IEnumerable<GatewayResponseDto>> GetAllByParkingId(long parkingId);
    Task<GatewayResponseDto> AddAsync(GatewayRequestDto dto);
    Task<GatewayResponseDto> UpdateAsync(long id, GatewayRequestDto dto);
    Task RemoveAsync(long id);
}