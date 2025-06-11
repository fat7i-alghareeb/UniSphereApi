using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class EnrollmentStatusConfiguration : IEntityTypeConfiguration<EnrollmentStatus>
{
    public void Configure(EntityTypeBuilder<EnrollmentStatus> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.HasMany(e => e.StudentCredentials)
            .WithOne(s => s.EnrollmentStatus)
            .HasForeignKey(s => s.EnrollmentStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 