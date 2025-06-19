using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Configurations;

public class StudentStatisticsConfiguration
{
    public void Configure(EntityTypeBuilder<StudentStatistics> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Average).IsRequired();
        builder.Property(s => s.NumberOfAttendanceHours).IsRequired();
        builder.Property(s => s.NumberOfAttendanceLectures).IsRequired();
        builder.HasOne(s => s.Student)
            .WithOne(s => s.StudentStatistics)
            .HasForeignKey<StudentStatistics>(s => s.StudentId)
            
            .OnDelete(DeleteBehavior.Restrict);
    }
}
