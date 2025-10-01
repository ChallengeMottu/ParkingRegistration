using PulseSystem.Domain.Entities;

namespace PulseSystem.Infraestructure.Repositories.interfaces;

public interface IParkingRepository : IMethodsRepository<Parking>
{
    Task<IEnumerable<Parking>> GetAllOrderByCapacityDescAsync();
    Task<IEnumerable<Parking>> GetAllOrderByAvailableAreaDescAsync();
    Task<Parking?> GetByLocationAsync(string street, string complement);
    
    Task<IEnumerable<Parking>> GetAllPaginatedAsync(int page, int pageSize);


}