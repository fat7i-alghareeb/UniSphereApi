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
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(i => i.LastName)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.HasMany(i => i.InstructorLabs)
            .WithOne(il => il.Instructor)
            .HasForeignKey(il => il.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 