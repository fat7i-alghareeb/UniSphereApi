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
            
        builder.Property(l => l.SubjectId)
            .IsRequired();
            
        builder.Property(l => l.ProfessorId)
            .IsRequired();
            
        builder.Property(l => l.StartTime)
            .IsRequired();
            
        builder.Property(l => l.EndTime)
            .IsRequired();
            
        builder.Property(l => l.LectureHall)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.HasOne(l => l.Schedule)
            .WithMany(s => s.Lectures)
            .HasForeignKey(l => l.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(l => l.Subject)
            .WithMany(s => s.Lectures)
            .HasForeignKey(l => l.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(l => l.Professor)
            .WithMany(p => p.Lectures)
            .HasForeignKey(l => l.ProfessorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 