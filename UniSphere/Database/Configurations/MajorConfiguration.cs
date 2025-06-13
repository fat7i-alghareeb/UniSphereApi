using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class MajorConfiguration : IEntityTypeConfiguration<Major>
{
    public void Configure(EntityTypeBuilder<Major> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Name)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(m => m.FacultyId)
            .IsRequired();
            
        builder.Property(m => m.NumberOfYears)
            .IsRequired();
            
        builder.HasOne(m => m.Faculty)
            .WithMany(f => f.Majors)
            .HasForeignKey(m => m.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(m => m.Subjects)
            .WithOne(s => s.Major)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(m => m.StudentCredentials)
            .WithOne(s => s.Major)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(m => m.Schedules)
            .WithOne(s => s.Major)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(m => m.MajorAnnouncements)
            .WithOne(ma => ma.Major)
            .HasForeignKey(ma => ma.MajorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
