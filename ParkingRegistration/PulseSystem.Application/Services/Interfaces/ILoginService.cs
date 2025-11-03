using PulseSystem.Domain.Entities;

namespace PulseSystem.Application.Services.interfaces;

public interface ILoginService
{

    Task<Employee?> ValidatedUserAsync(string email, string password);
    bool IsGestor(Employee employee);
}