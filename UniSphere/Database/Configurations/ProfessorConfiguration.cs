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
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasMany(p => p.SubjectLecturers)
            .WithOne(sl => sl.Professor)
            .HasForeignKey(sl => sl.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 