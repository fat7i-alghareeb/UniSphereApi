using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class StudentCredentialConfiguration : IEntityTypeConfiguration<StudentCredential>
{
    public void Configure(EntityTypeBuilder<StudentCredential> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.StudentNumber)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(s => s.Image)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(s => s.FirstName)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(s => s.OneTimeCode);
        builder.Property(s => s.OneTimeCodeExpirationInMinutes);
        builder.Property(s => s.OneTimeCodeCreatedDate);
        builder.Property(s => s.LastName)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(s => s.FatherName)
            .HasColumnType("jsonb");

        builder.Property(s => s.Year)
            .IsRequired();


        builder.Property(s => s.IdentityId)
            .HasMaxLength(500);


        builder.Property(s => s.MajorId)
            .IsRequired();

        builder.Property(s => s.EnrollmentStatusId)
            .IsRequired();

        builder.HasOne(s => s.Major)
            .WithMany(m => m.StudentCredentials)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.EnrollmentStatus)
            .WithMany(e => e.StudentCredentials)
            .HasForeignKey(s => s.EnrollmentStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.StudentStatistics)
            .WithOne(ss => ss.Student)
            .HasForeignKey<StudentStatistics>(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(s => s.SubjectStudentLinks)
            .WithOne(ssl => ssl.StudentCredential)
            .HasForeignKey(ssl => ssl.StudentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(s => s.IdentityId).IsUnique();
        builder.HasIndex(s => new { s.StudentNumber, s.MajorId }).IsUnique();
    }
}
