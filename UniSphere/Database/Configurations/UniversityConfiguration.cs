using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class UniversityConfiguration : IEntityTypeConfiguration<University>
{
    public void Configure(EntityTypeBuilder<University> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Name)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(u => u.Type)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.HasMany(u => u.Faculties)
            .WithOne(f => f.University)
            .HasForeignKey(f => f.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 