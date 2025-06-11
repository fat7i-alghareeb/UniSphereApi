using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class SubjectLecturerConfiguration : IEntityTypeConfiguration<SubjectLecturer>
{
    public void Configure(EntityTypeBuilder<SubjectLecturer> builder)
    {
        builder.HasKey(sl => new { sl.SubjectId, sl.ProfessorId });
        
        builder.Property(sl => sl.SubjectId)
            .IsRequired();
            
        builder.Property(sl => sl.ProfessorId)
            .IsRequired();
            
        builder.HasOne(sl => sl.Subject)
            .WithMany(s => s.SubjectLecturers)
            .HasForeignKey(sl => sl.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(sl => sl.Professor)
            .WithMany(p => p.SubjectLecturers)
            .HasForeignKey(sl => sl.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 