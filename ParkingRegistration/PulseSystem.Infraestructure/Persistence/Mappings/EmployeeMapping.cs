using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseSystem.Domain.Entities;

namespace PulseSystem.Infraestructure.Persistence.Mappings;

public class EmployeeMapping : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("EMPLOYEES", "RM558830");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("ID");
        builder.Property(e => e.Email).HasColumnName("EMAIL");
        builder.Property(e => e.Password).HasColumnName("PASSWORD");
        builder.Property(e => e.Role).HasColumnName("ROLE");
        
    }
}