using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.SubjectId)
            .IsRequired();
            
        builder.Property(m => m.Url)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(m => m.CreatedAt)
            .IsRequired();
            
        builder.HasOne(m => m.Subject)
            .WithMany(s => s.Materials)
            .HasForeignKey(m => m.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 