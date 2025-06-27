using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class SuperAdminConfiguration : IEntityTypeConfiguration<SuperAdmin>
{
    public void Configure(EntityTypeBuilder<SuperAdmin> builder)
    {
        builder.HasKey(sa => sa.Id);

        builder.Property(sa => sa.FirstName)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(sa => sa.LastName)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(sa => sa.Gmail)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(sa => sa.OneTimeCode);
        builder.Property(sa => sa.OneTimeCodeExpirationInMinutes);
        builder.Property(sa => sa.OneTimeCodeCreatedDate);
        builder.Property(sa => sa.FacultyId).IsRequired();
        builder.Property(sa => sa.Image)
            .HasMaxLength(500);

        builder.HasOne(sa => sa.Faculty)
            .WithMany(f => f.SuperAdmins)
            .HasForeignKey(sa => sa.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 