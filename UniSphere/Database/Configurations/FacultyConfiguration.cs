using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
    {
        builder.HasKey(f => f.Id);
        
        builder.Property(f => f.Name)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(f => f.UniversityId)
            .IsRequired();
        builder.Property(f => f.DaysToTheFinale)
            .IsRequired();
            
        builder.HasOne(f => f.University)
            .WithMany(u => u.Faculties)
            .HasForeignKey(f => f.UniversityId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(f => f.Majors)
            .WithOne(m => m.Faculty)
            .HasForeignKey(m => m.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(f => f.FacultyAnnouncements)
            .WithOne(fa => fa.Faculty)
            .HasForeignKey(fa => fa.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(f => f.ProfessorFacultyLinks)
            .WithOne(pfl => pfl.Faculty)
            .HasForeignKey(pfl => pfl.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(f => f.SuperAdmins)
            .WithOne(sa => sa.Faculty)
            .HasForeignKey(sa => sa.FacultyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 
