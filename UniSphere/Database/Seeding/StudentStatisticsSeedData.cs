using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class StudentStatisticsSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.StudentStatistics.AnyAsync())
        {
            var studentStatistics = new List<StudentStatistics>
            {
                new()
                {
                    Id = Guid.Parse("44444444-9832-1234-4444-444444944453"),
                    StudentId = Guid.Parse("0c577686-efd9-40ab-b454-c5bbac8a4c95"),
                    Average = 85.5,
                    NumberOfAttendanceHours = 10,
                    NumberOfAttendanceLectures = 5,

                }
                
            };

            await Context.StudentStatistics.AddRangeAsync(studentStatistics);
            await Context.SaveChangesAsync();
        }
    }
}
