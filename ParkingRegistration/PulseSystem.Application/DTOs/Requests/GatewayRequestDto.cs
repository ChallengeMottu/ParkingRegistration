using System.ComponentModel.DataAnnotations;
using PulseSystem.Domain.Enums;

namespace PulseSystem.Application.DTOs.requests;

public class GatewayRequestDto
{
    
    [Required(ErrorMessage = "O campo Modelo é obrigatório.")]
    public string Model {get; set;}
    
    [Required(ErrorMessage = "O Status é obrigatório.")]
    public StatusGateway Status {get; set;}
    
    [Required(ErrorMessage = "O MAC Address é obrigatório.")]
    public string MacAddress {get; set;}
    
    [Required(ErrorMessage = "O IP é obrigatório.")]
    public string LastIP {get; set;}
    
    [Required(ErrorMessage = "O ID do pátio precisa ser informado")]
    public long ParkingId { get; set; }
}