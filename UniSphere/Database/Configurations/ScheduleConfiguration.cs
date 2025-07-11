using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.MajorId)
            .IsRequired();
            
        builder.Property(s => s.Year)
            .IsRequired();
            
        builder.Property(s => s.ScheduleDate)
            .IsRequired();
            
        builder.HasOne(s => s.Major)
            .WithMany(m => m.Schedules)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(s => s.Lectures)
            .WithOne(l => l.Schedule)
            .HasForeignKey(l => l.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(s => s.ScheduleLabLinks)
            .WithOne(l => l.Schedule)
            .HasForeignKey(l => l.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(s => new { s.MajorId, s.Year, s.ScheduleDate }).IsUnique();
    }
} 
