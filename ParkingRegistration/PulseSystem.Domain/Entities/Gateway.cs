using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using PulseSystem.Domain.Enums;

namespace PulseSystem.Domain.Entities;

public class Gateway
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    
    public string Model {get; set;}
    
    
    public StatusGateway Status {get; set;}
    
    
    public string MacAddress {get; set;}
    
   
    public string LastIP {get; set;}
    
    
    public DateTime RegisterDate {get; set;} = DateTime.Now;
    
    public long ParkingId { get; set; }
    
    public Parking Parking { get; set; }
    
    
    
    public decimal MaxCoverageArea { get; set; } = 10000m; 
    public int MaxCapacity { get; set; } = 100;           
       
    
    public bool IsValidMacAddress()
    {
        if (string.IsNullOrWhiteSpace(MacAddress)) return false;
        var regex = new Regex(@"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$");
        return regex.IsMatch(MacAddress);
    }

    
    public bool IsValidIp()
    {
        if (string.IsNullOrWhiteSpace(LastIP)) return false;
        var regex = new Regex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\.|$)){4}$");
        return regex.IsMatch(LastIP);
    }

    
    public bool IsValid()
    {
        return IsValidMacAddress() && IsValidIp();
    }
}