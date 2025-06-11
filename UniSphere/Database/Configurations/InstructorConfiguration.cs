using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.HasKey(i => i.Id);
        
        builder.Property(i => i.FirstName)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(i => i.LastName)
            .HasColumnType("jsonb")
            .IsRequired();
            
        builder.Property(i => i.FatherName)
            .HasColumnType("jsonb");
            
        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.Property(i => i.Password)
            .IsRequired()
            .HasMaxLength(256);
            
        builder.HasMany(i => i.InstructorLabLinks)
            .WithOne(ill => ill.Instructor)
            .HasForeignKey(ill => ill.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 