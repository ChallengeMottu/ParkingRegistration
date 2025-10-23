using System.ComponentModel.DataAnnotations;

namespace PulseSystem.Domain.Entities;

public class Employee
{
    [Key]
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}