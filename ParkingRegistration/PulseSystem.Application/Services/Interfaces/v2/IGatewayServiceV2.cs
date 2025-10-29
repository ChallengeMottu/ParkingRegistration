using PulseSystem.Application.DTOs.requests;
using PulseSystem.Application.DTOs.responses;

namespace PulseSystem.Application.Services.interfaces.v2;

public interface IGatewayServiceV2
{
    Task<GatewayResponseDto> GetByIdAsync(long id);
    Task<GatewayResponseDto> AddAsync(GatewayRequestDto dto);
    Task<GatewayResponseDto> UpdateAsync(long id, GatewayRequestDto dto);
    

}