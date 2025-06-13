using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class FacultyAnnouncementConfiguration : IEntityTypeConfiguration<FacultyAnnouncement>
{
    public void Configure(EntityTypeBuilder<FacultyAnnouncement> builder)
    {
        builder.HasKey(fa => fa.Id);

        builder.Property(fa => fa.Title)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(fa => fa.Content)
            .HasColumnType("jsonb");

        builder.Property(fa => fa.CreatedAt)
            .IsRequired();

        builder.HasIndex(fa => fa.CreatedAt);
    }
}