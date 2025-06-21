using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class ScheduleLabLinkConfiguration: IEntityTypeConfiguration<ScheduleLabLink>
{
    public void Configure(EntityTypeBuilder<ScheduleLabLink> builder)
    {
        builder.HasKey(sl => new{sl.ScheduleId, sl.LabId});
        
        
        builder.HasOne(sl => sl.Schedule)
            .WithMany(s => s.ScheduleLabLinks)
            .HasForeignKey(sl => sl.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(sl => sl.Lab)
            .WithMany(l => l.ScheduleLabLinks)
            .HasForeignKey(sl => sl.LabId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
}
