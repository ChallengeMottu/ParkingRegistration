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
            builder.Property(g => g.Id).HasColumnName("ID");
            
            builder.Property(g => g.Model)
                .IsRequired()
                .HasColumnName("MODEL")
                .HasMaxLength(100)
                .HasColumnType("VARCHAR2(100)"); 

            builder.Property(g => g.Status)
                .IsRequired()
                .HasColumnName("STATUS")
                .HasColumnType("NUMBER"); 

            builder.Property(g => g.MacAddress)
                .IsRequired()
                .HasColumnName("MAC_ADDRESS")
                .HasMaxLength(17)
                .HasColumnType("VARCHAR2(17)"); 

            builder.Property(g => g.LastIP)
                .IsRequired()
                .HasColumnName("LAST_IP")
                .HasMaxLength(15)
                .HasColumnType("VARCHAR2(15)"); 

            builder.Property(g => g.RegisterDate)
                .IsRequired()
                .HasColumnName("REGISTER_DATE")
                .HasDefaultValueSql("SYSDATE");  

            builder.Property(g => g.MaxCoverageArea)
                .IsRequired()
                .HasColumnName("MAX_COVERAGE_AREA")
                .HasColumnType("NUMBER"); 

            builder.Property(g => g.MaxCapacity)
                .IsRequired()
                .HasColumnName("MAX_CAPACITY")
                .HasColumnType("NUMBER"); 


            
            builder.HasOne(g => g.Parking)
                .WithMany(p => p.Gateways)
                .HasForeignKey(g => g.ParkingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}