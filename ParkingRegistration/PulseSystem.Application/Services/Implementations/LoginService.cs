using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Repositories.interfaces;

namespace PulseSystem.Application.Services.Implementations;

public class LoginService
{
    private readonly IEmployeeRepository _employeeRepository;

    public LoginService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Employee?> ValidatedUserAsync(string email, string password)
    {
        var employee = await _employeeRepository.GetByEmailAsync(email);
        if (employee == null)
            return null;
        if (!BCrypt.Net.BCrypt.Verify(password, employee.Password))
        {
            return null;
        }

        return new Employee
        {
            Email = employee.Email,
            Role = employee.Role
        };
    }

    public bool IsGestor(Employee employee)=>
        employee.Role.Equals("GESTOR",  StringComparison.OrdinalIgnoreCase);
   
}