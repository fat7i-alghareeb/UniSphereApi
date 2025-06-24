using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class ProfessorFacultyLinkConfiguration : IEntityTypeConfiguration<ProfessorFacultyLink>
{
    public void Configure(EntityTypeBuilder<ProfessorFacultyLink> builder)
    {
        builder.HasKey(pfl => new { pfl.ProfessorId, pfl.FacultyId });
        
        builder.Property(pfl => pfl.ProfessorId)
            .IsRequired();
            
        builder.Property(pfl => pfl.FacultyId)
            .IsRequired();
            
        builder.HasOne(pfl => pfl.Professor)
            .WithMany(p => p.ProfessorFacultyLinks)
            .HasForeignKey(pfl => pfl.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pfl => pfl.Faculty)
            .WithMany(f => f.ProfessorFacultyLinks)
            .HasForeignKey(pfl => pfl.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 