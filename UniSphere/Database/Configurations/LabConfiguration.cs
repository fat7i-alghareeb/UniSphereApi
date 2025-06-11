using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class LabConfiguration : IEntityTypeConfiguration<Lab>
{
    public void Configure(EntityTypeBuilder<Lab> builder)
    {
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.Name)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(l => l.Description)
            .HasColumnType("jsonb");
            
        builder.HasMany(l => l.InstructorLabLinks)
            .WithOne(il => il.Lab)
            .HasForeignKey(il => il.LabId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(l => l.Subjects)
            .WithOne(s => s.Lab)
            .HasForeignKey(s => s.LabId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 