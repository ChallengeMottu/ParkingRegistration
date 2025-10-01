using System.ComponentModel.DataAnnotations;

namespace PulseSystem.Application.DTOs.requests;

public class ZoneRequestDto
{
    [Required(ErrorMessage = "O campo nome é obrigatório")]
    public string Name { get; set; }
    public string Description { get; set; }
    
    [Required(ErrorMessage = "A largura precisa ser informada")]
    public decimal Width { get; set; }
    
    [Required(ErrorMessage = "O comprimento precisa ser informado")]
    public decimal Length { get; set; }
    
    [Required(ErrorMessage = "O ID do pátio é obrigatório")]
    public long ParkingId { get; set; }
}