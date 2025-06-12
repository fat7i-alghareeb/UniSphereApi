using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class InstructorLabLinkSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.InstructorLabLink.AnyAsync())
        {
            List<Instructor> instructors = await Context.Instructors.ToListAsync();
            List<Lab> labs = await Context.Labs.ToListAsync();
            if(instructors.Count == 0 || labs.Count == 0)
            {
                return;
            }
            var links = new List<InstructorLabLink>
            {
                new() { InstructorId = instructors[0].Id, LabId = labs[0].Id },
                new() { InstructorId = instructors[1].Id, LabId = labs[1].Id }
            };
            await Context.InstructorLabLink.AddRangeAsync(links);
            await Context.SaveChangesAsync();
        }
    }
} 
