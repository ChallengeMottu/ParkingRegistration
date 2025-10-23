using PulseSystem.Application.DTOs.Hateoas;
using PulseSystem.Domain.Enums;

namespace PulseSystem.Application.DTOs.responses;

public class GatewayResponseDto
{
    public long Id { get; set; }
    
    
    public string Model {get; set;}
    
    
    public StatusGateway Status {get; set;}
    
    
    public string MacAddress {get; set;}
    
   
    public string LastIP {get; set;}
    
    
    public DateTime RegisterDate {get; set;} = DateTime.Now;
    
    public long ParkingId { get; set; }
    
    public List<LinkDto> Links { get; set; } = new List<LinkDto>();
}