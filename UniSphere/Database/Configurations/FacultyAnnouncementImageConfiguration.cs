using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class FacultyAnnouncementImageConfiguration : IEntityTypeConfiguration<FacultyAnnouncementImage>
{
    public void Configure(EntityTypeBuilder<FacultyAnnouncementImage> builder)
    {
        builder.HasKey(fai => fai.Id);
        
        builder.Property(fai => fai.FacultyAnnouncementId)
            .IsRequired();
            
        builder.Property(fai => fai.Url)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(fai => fai.CreatedAt)
            .IsRequired();
            
        builder.HasOne(fai => fai.FacultyAnnouncement)
            .WithMany(fa => fa.Images)
            .HasForeignKey(fai => fai.FacultyAnnouncementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 