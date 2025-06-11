using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(s => s.Description)
            .HasColumnType("jsonb");
            
        builder.Property(s => s.MajorId)
            .IsRequired();
            
        builder.Property(s => s.IsLabRequired)
            .HasDefaultValue(false)
            .IsRequired();
            
        builder.Property(s => s.IsMultipleChoice)
            .HasDefaultValue(false)
            .IsRequired();
            
        builder.Property(s => s.IsOpenBook)
            .HasDefaultValue(false)
            .IsRequired();
            
        builder.Property(s => s.MidtermGrade)
            .HasDefaultValue(30);
            
        builder.Property(s => s.FinalGrade)
            .HasDefaultValue(70);
            
        builder.HasOne(s => s.Major)
            .WithMany(m => m.Subjects)
            .HasForeignKey(s => s.MajorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(s => s.Lab)
            .WithMany(l => l.Subjects)
            .HasForeignKey(s => s.LabId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(s => s.SubjectLecturers)
            .WithOne(sl => sl.Subject)
            .HasForeignKey(sl => sl.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(s => s.SubjectStudentLinks)
            .WithOne(ssl => ssl.Subject)
            .HasForeignKey(ssl => ssl.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
