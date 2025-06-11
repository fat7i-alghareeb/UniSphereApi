using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class StudentCredentialConfiguration : IEntityTypeConfiguration<StudentCredential>
{
    public void Configure(EntityTypeBuilder<StudentCredential> builder)
    {
        builder.HasKey(s => new { s.Id, s.FacultyId });
        
        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.Password)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.FirstName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.FatherName)
            .HasMaxLength(50);
            
        builder.Property(s => s.Year)
            .IsRequired();
            
        builder.Property(s => s.MajorId)
            .IsRequired();
            
        builder.Property(s => s.EnrollmentStatusId)
            .IsRequired();
            
        // Unique index on Email
        builder.HasIndex(s => s.Email)
            .IsUnique();
            
        builder.HasOne(s => s.Faculty)
            .WithMany(f => f.StudentCredentials)
            .HasForeignKey(s => s.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(s => s.Major)
            .WithMany(m => m.StudentCredentials)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(s => s.EnrollmentStatus)
            .WithMany(e => e.StudentCredentials)
            .HasForeignKey(s => s.EnrollmentStatusId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(s => s.SubjectStudentLinks)
            .WithOne(l => l.StudentCredential)
            .HasForeignKey(l => new { l.StudentId, l.FacultyId })
            .OnDelete(DeleteBehavior.Cascade);
    }
} 