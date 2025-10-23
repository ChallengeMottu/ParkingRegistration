using Microsoft.EntityFrameworkCore;
using PulseSystem.Domain.Entities;
using PulseSystem.Infraestructure.Persistence.Mappings;

namespace PulseSystem.Infraestructure.Persistence;

public class PulseSystemContext : DbContext
{

    public PulseSystemContext(DbContextOptions<PulseSystemContext> options) : base(options)
    {
        
    }
    
    public DbSet<Parking>  Parkings { get; set; }
    public DbSet<Zone> Zones { get; set; }
    public DbSet<Gateway>  Gateways { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ParkingMapping());
        modelBuilder.ApplyConfiguration(new ZoneMapping());
        modelBuilder.ApplyConfiguration(new GatewayMapping());
        modelBuilder.ApplyConfiguration(new EmployeeMapping());
        base.OnModelCreating(modelBuilder);
    }
    
}