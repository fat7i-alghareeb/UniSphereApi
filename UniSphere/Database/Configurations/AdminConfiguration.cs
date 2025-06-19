using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FirstName)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(a => a.LastName)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(a => a.Gmail)
            .IsRequired()
            .HasMaxLength(256);
        builder.Property(a => a.OneTimeCode);
        builder.Property(a => a.OneTimeCodeExpirationInMinutes);
        builder.Property(a => a.OneTimeCodeCreatedDate);
        builder.Property(a => a.MajorId).IsRequired();

        builder.HasOne(a => a.Major)
            .WithMany()
            .HasForeignKey(a => a.MajorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
} 