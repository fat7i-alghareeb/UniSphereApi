using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class SubjectProfessorLinkConfiguration : IEntityTypeConfiguration<SubjectProfessorLink>
{
    public void Configure(EntityTypeBuilder<SubjectProfessorLink> builder)
    {
        builder.HasKey(sl => new { sl.SubjectId, sl.ProfessorId });
        
        builder.Property(sl => sl.SubjectId)
            .IsRequired();
            
        builder.Property(sl => sl.ProfessorId)
            .IsRequired();
            
        builder.HasOne(sl => sl.Subject)
            .WithMany(s => s.SubjectLecturers)
            .HasForeignKey(sl => sl.SubjectId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(sl => sl.Professor)
            .WithMany(p => p.SubjectProfessorLinks)
            .HasForeignKey(sl => sl.ProfessorId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 