using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class InstructorLabConfiguration : IEntityTypeConfiguration<InstructorLab>
{
    public void Configure(EntityTypeBuilder<InstructorLab> builder)
    {
        builder.HasKey(il => new { il.InstructorId, il.LabId });
        
        builder.Property(il => il.InstructorId)
            .IsRequired();
            
        builder.Property(il => il.LabId)
            .IsRequired();
            
        builder.HasOne(il => il.Instructor)
            .WithMany(i => i.InstructorLabs)
            .HasForeignKey(il => il.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(il => il.Lab)
            .WithMany(l => l.InstructorLabs)
            .HasForeignKey(il => il.LabId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 