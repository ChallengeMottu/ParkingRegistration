using PulseSystem.Domain.Entities;
using PulseSystem.Domain.Enums;

namespace PulseSystem.Infraestructure.Repositories.interfaces;

public interface IZoneRepository : IMethodsRepository<Zone>
{
    Task<IEnumerable<Zone>> GetByParkingIdAsync(long parkingId);
    Task<int> CountByParkingIdAsync(long parkingId);
}