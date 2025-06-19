using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(p => p.LastName)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(p => p.Bio)
            .HasColumnType("jsonb")
            .IsRequired();
        builder.Property(s => s.Gmail).HasMaxLength(256);
        builder.Property(s => s.OneTimeCode);
        builder.Property(s => s.OneTimeCodeExpirationInMinutes);
        builder.Property(s => s.OneTimeCodeCreatedDate);
        builder.HasMany(p => p.SubjectProfessorLinks)
            .WithOne(sl => sl.Professor)
            .HasForeignKey(sl => sl.ProfessorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
