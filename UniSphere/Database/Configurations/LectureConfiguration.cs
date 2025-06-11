using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class LectureConfiguration : IEntityTypeConfiguration<Lecture>
{
    public void Configure(EntityTypeBuilder<Lecture> builder)
    {
        builder.HasKey(l => l.Id);
        
        builder.Property(l => l.ScheduleId)
            .IsRequired();
            
        builder.Property(l => l.SubjectName)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(l => l.LecturerName)
            .HasColumnType("jsonb");
            
        builder.Property(l => l.StartTime)
            .IsRequired();
            
        builder.Property(l => l.EndTime)
            .IsRequired();
            
        builder.Property(l => l.LectureHall)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.HasOne(l => l.Schedule)
            .WithMany(s => s.Lectures)
            .HasForeignKey(l => l.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 