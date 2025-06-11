// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using UniSphere.Api.Entities;

// namespace UniSphere.Api.Database.Configurations;

// public class MultilingualTextConfiguration : IEntityTypeConfiguration<MultilingualText>
// {
//     public void Configure(EntityTypeBuilder<MultilingualText> builder)
//     {
//         builder.ToTable("MultilingualTexts");
        
//         builder.Property(mt => mt.En)
//             .IsRequired()
//             .HasDefaultValue(string.Empty);
            
//         builder.Property(mt => mt.Ar)
//             .IsRequired()
//             .HasDefaultValue(string.Empty);
//     }
// } 