using System.ComponentModel.DataAnnotations;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Application.DTOs.requests;

public class ParkingRequestDto
{
    
    [Required(ErrorMessage = "O campo nome é obrigatório")]
    public string Name { get; set; }
    
    
    public Address Location { get; set; }
    
    [Required(ErrorMessage = "A área disponível precisa ser informada")]
    public decimal AvailableArea { get; set; }
    
    [Required(ErrorMessage = "A capacidade precisa ser informada")]
    public int Capacity { get; set; }
    
    
    [Required(ErrorMessage = "A planta da estrutura do pátio é obrigatória")]
    public string structurePlan { get; set; }
    
    
    [Required(ErrorMessage = "A planta baixa do pátio completa é obrigatória")]
    public string floorPlan { get; set; } 
}