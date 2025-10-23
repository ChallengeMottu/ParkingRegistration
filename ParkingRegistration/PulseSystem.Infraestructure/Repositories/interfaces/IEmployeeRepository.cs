using PulseSystem.Domain.Entities;

namespace PulseSystem.Infraestructure.Repositories.interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByEmailAsync(string email);
}