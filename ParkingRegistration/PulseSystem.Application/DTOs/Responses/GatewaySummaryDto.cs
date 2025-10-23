using PulseSystem.Domain.Enums;

namespace PulseSystem.Application.DTOs.responses;

public class GatewaySummaryDto
{
    public long Id { get; set; }
    
    
    public string Model {get; set;}
    
    
    public StatusGateway Status {get; set;}
    
    
    public string MacAddress {get; set;}
    
   
    public string LastIP {get; set;}
}