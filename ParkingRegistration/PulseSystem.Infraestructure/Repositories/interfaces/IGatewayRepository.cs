using PulseSystem.Domain.Entities;
using PulseSystem.Domain.Enums;

namespace PulseSystem.Infraestructure.Repositories.interfaces;

public interface IGatewayRepository : IMethodsRepository<Gateway>
{
    Task<Gateway?> GetByMacAddressAsync(string macAddress);
    Task<IEnumerable<Gateway>> GetAllByParkingId(long parkingId);
    
}