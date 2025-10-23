using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Infraestructure.Persistence.Mappings
{
    public class GatewayMapping : IEntityTypeConfiguration<Gateway>
    {
        public void Configure(EntityTypeBuilder<Gateway> builder)
        {
            
            builder.ToTable("GATEWAYS");

            
            builder.HasKey(g => g.Id);

            
            builder.Property(g => g.Model)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("VARCHAR2(100)"); 

            builder.Property(g => g.Status)
                .IsRequired()
                .HasColumnType("NUMBER"); 

            builder.Property(g => g.MacAddress)
                .IsRequired()
                .HasMaxLength(17)
                .HasColumnType("VARCHAR2(17)"); 

            builder.Property(g => g.LastIP)
                .IsRequired()
                .HasMaxLength(15)
                .HasColumnType("VARCHAR2(15)"); 

            builder.Property(g => g.RegisterDate)
                .IsRequired()
                .HasDefaultValueSql("SYSDATE");  

            builder.Property(g => g.MaxCoverageArea)
                .IsRequired()
                .HasColumnType("NUMBER"); 

            builder.Property(g => g.MaxCapacity)
                .IsRequired()
                .HasColumnType("NUMBER"); 


            
            builder.HasOne(g => g.Parking)
                .WithMany(p => p.Gateways)
                .HasForeignKey(g => g.ParkingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}