using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Infraestructure.Persistence.Mappings
{
    public class ZoneMapping : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            
            builder.ToTable("ZONES");

            
            builder.HasKey(z => z.Id);

            builder.Property(z => z.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("VARCHAR2(100)"); 

            builder.Property(z => z.Description)
                .HasMaxLength(500)
                .HasColumnType("VARCHAR2(500)"); 

            builder.Property(z => z.Width)
                .IsRequired()
                .HasColumnType("NUMBER"); 

            builder.Property(z => z.Length)
                .IsRequired()
                .HasColumnType("NUMBER"); 


            builder.HasOne(z => z.Parking)
                .WithMany(p => p.Zones)
                .HasForeignKey(z => z.ParkingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}