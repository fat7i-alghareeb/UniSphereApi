using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class SubjectStudentLinkConfiguration : IEntityTypeConfiguration<SubjectStudentLink>
{
    public void Configure(EntityTypeBuilder<SubjectStudentLink> builder)
    {
        builder.HasKey(ssl => new { ssl.SubjectId, ssl.StudentId});
        
        builder.Property(ssl => ssl.AttemptNumber)
            .IsRequired()
            .HasDefaultValue(1);
        builder.Property(ssl => ssl.AttemptNumber)
            .IsRequired()
            .HasDefaultValue(1);
        builder.Property(ssl => ssl.IsPassed)
            .IsRequired()
            .HasDefaultValue(false);
        builder.Property(ssl => ssl.MidtermGrade)
            .IsRequired(false);
            
        builder.Property(ssl => ssl.FinalGrade)
            .IsRequired(false);
            
        builder.Property(ssl => ssl.IsCurrentlyEnrolled)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(ssl => ssl.Notes)
            .HasColumnType("jsonb");
            
        builder.HasOne(ssl => ssl.Subject)
            .WithMany(s => s.SubjectStudentLinks)
            .HasForeignKey(ssl => ssl.SubjectId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(ssl => ssl.StudentCredential)
            .WithMany(sc => sc.SubjectStudentLinks)
            .HasForeignKey(ssl => ssl.StudentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
