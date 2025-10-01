using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseSystem.Domain.Entities;

public class Zone
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    
    public long ParkingId { get; set; }
    
    public Parking Parking { get; set; }
    
    public bool IsValidDimensions() => Width > 0 && Length > 0;
    
}