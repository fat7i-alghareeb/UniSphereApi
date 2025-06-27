using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class SystemControllerConfiguration : IEntityTypeConfiguration<SystemController>
{
    public void Configure(EntityTypeBuilder<SystemController> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Gmail).HasMaxLength(255).IsRequired();
        builder.Property(e => e.UserName).HasMaxLength(255).IsRequired();
        builder.Property(e => e.Image).HasMaxLength(500);
    }
}