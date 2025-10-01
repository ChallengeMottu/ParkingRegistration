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

            
            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(150)
                   .HasColumnType("VARCHAR2(150)"); 

            builder.Property(p => p.AvailableArea)
                   .IsRequired()
                   .HasColumnType("NUMBER"); 

            builder.Property(p => p.Capacity)
                   .IsRequired()
                   .HasColumnType("NUMBER"); 

            builder.Property(p => p.RegisterDate)
                   .IsRequired()
                   .HasDefaultValueSql("SYSDATE"); 


            
            builder.OwnsOne(x => x.Location, address =>

            {

                   address.Property(e => e.Street).HasColumnName("Street").HasMaxLength(100).IsRequired();

                   address.Property(e => e.Complement).HasColumnName("Complement").HasMaxLength(50);

                   address.Property(e => e.Neighborhood).HasColumnName("Neighborhood").HasMaxLength(100).IsRequired();

                   address.Property(e => e.City).HasColumnName("City").HasMaxLength(100).IsRequired();

                   address.Property(e => e.State).HasColumnName("State").HasMaxLength(50).IsRequired();

                   address.Property(e => e.Cep).HasColumnName("Cep").HasMaxLength(9).IsRequired();

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
