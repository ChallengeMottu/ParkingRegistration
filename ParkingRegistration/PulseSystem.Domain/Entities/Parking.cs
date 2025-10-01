using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PulseSystem.Domain.Entities;

public class Parking
{
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public Address Location { get; set; }
    public decimal AvailableArea { get; set; }
    public int Capacity { get; set; }
    public DateTime RegisterDate { get; set; } = DateTime.Now;
    public List<Zone> Zones { get; set; } = new List<Zone>();
    public List<Gateway> Gateways { get; set; } = new List<Gateway>();
    
    private const int MaxZones = 4;
    
    
    public int CalculateRequiredGateways(decimal maxCoveragePerGateway, int maxCapacityPerGateway)
    {
        int areaGateways = (int)Math.Ceiling(this.AvailableArea / maxCoveragePerGateway);
        int capacityGateways = (int)Math.Ceiling((decimal)this.Capacity / maxCapacityPerGateway);

        return Math.Max(areaGateways, capacityGateways);
    }
    
   
    public bool HasReachedMaxZones() => Zones.Count >= MaxZones;

   
    public bool ExceedsAvailableArea(Zone newZone)
    {
        var totalArea = Zones.Sum(z => z.Width * z.Length) + (newZone.Width * newZone.Length);
        return totalArea > AvailableArea;
    }
    
    
    
    
}
