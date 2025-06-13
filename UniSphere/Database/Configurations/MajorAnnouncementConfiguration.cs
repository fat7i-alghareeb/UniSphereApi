using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class MajorAnnouncementConfiguration : IEntityTypeConfiguration<MajorAnnouncement>
{
    public void Configure(EntityTypeBuilder<MajorAnnouncement> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(m => m.Content)
            .HasColumnType("jsonb");

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.HasIndex(m => m.CreatedAt);
    }
}