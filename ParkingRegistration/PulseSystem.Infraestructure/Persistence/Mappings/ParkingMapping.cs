using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Infraestructure.Persistence.Mappings
{
    public class ParkingMapping : IEntityTypeConfiguration<Parking>
    {
        public void Configure(EntityTypeBuilder<Parking> builder)
        {
            
            builder.ToTable("PARKINGS");

            
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("ID");

            
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasColumnName("NAME")
                   .HasMaxLength(150)
                   .HasColumnType("VARCHAR2(150)"); 

            builder.Property(p => p.AvailableArea)
                   .HasColumnName("AVAILABLE_AREA")
                   .IsRequired()
                   .HasColumnType("NUMBER"); 

            builder.Property(p => p.Capacity)
                   .HasColumnName("CAPACITY")
                   .IsRequired()
                   .HasColumnType("NUMBER"); 

            builder.Property(p => p.RegisterDate)
                   .IsRequired()
                   .HasColumnName("REGISTER_DATE")
                   .HasDefaultValueSql("SYSDATE"); 
            
            builder.Property(p => p.StructurePlan)
                   .HasColumnName("STRUCTURE_PLAN")
                   .HasColumnType("VARCHAR2(4000)")
                   .IsRequired();
            
            builder.Property(p => p.FloorPlan)
                   .HasColumnName("FLOOR_PLAN")
                   .HasColumnType("VARCHAR2(4000)")
                   .IsRequired();
            
            builder.Property(p => p.MapPlan)
                   .HasColumnName("MAP_PLAN")
                   .HasColumnType("CLOB")
                   .IsRequired();
            
            builder.OwnsOne(x => x.Location, address =>

            {

                   address.Property(e => e.Street).HasColumnName("STREET").HasMaxLength(100).IsRequired();

                   address.Property(e => e.Complement).HasColumnName("COMPLEMENT").HasMaxLength(50);

                   address.Property(e => e.Neighborhood).HasColumnName("NEIGHBORHOOD").HasMaxLength(100).IsRequired();

                   address.Property(e => e.City).HasColumnName("CITY").HasMaxLength(100).IsRequired();

                   address.Property(e => e.State).HasColumnName("STATE").HasMaxLength(50).IsRequired();

                   address.Property(e => e.Cep).HasColumnName("CEP").HasMaxLength(9).IsRequired();

            });

            
            builder.HasMany(p => p.Zones)
                   .WithOne(z => z.Parking)
                   .HasForeignKey(z => z.ParkingId) 
                   .OnDelete(DeleteBehavior.Cascade);


            
            builder.HasMany(p => p.Gateways)
                   .WithOne(g => g.Parking)
                   .HasForeignKey(g => g.ParkingId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
