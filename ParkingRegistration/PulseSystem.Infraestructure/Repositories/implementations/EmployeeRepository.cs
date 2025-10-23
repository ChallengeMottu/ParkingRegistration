using Microsoft.EntityFrameworkCore;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Persistence;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Infraestructure.Repositories.implementations;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly PulseSystemContext _context;
    
    public EmployeeRepository(PulseSystemContext context) => _context = context;
    
    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e=> e.Email == email);
    }
}