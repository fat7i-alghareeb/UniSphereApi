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
                // Informatics Engineering Labs
                new() 
                { 
                    InstructorId = instructors[0].Id, // Rami Khaled
                    LabId = labs[0].Id // Advanced Programming Lab
                },
                new() 
                { 
                    InstructorId = instructors[1].Id, // Dina Saeed
                    LabId = labs[3].Id // Database Lab
                },
                new() 
                { 
                    InstructorId = instructors[2].Id, // Omar Hassan
                    LabId = labs[2].Id // AI Lab
                },

                // Electrical Engineering Labs
                new() 
                { 
                    InstructorId = instructors[3].Id, // Mohammed Ali
                    LabId = labs[4].Id // Electrical Circuits Lab
                },
                new() 
                { 
                    InstructorId = instructors[4].Id, // Sarah Ahmed
                    LabId = labs[5].Id // Electronics Lab
                },

                // Mechanical Engineering Labs
                new() 
                { 
                    InstructorId = instructors[5].Id, // Ali Hussein
                    LabId = labs[7].Id // Mechanical Design Lab
                },
                new() 
                { 
                    InstructorId = instructors[6].Id, // Fatima Mohammed
                    LabId = labs[6].Id // Communications Lab
                },

                // Civil Engineering Labs
                new() 
                { 
                    InstructorId = instructors[7].Id, // Khaled Ali
                    LabId = labs[8].Id // Construction Materials Lab
                },
                new() 
                { 
                    InstructorId = instructors[8].Id, // Nour Ahmed
                    LabId = labs[9].Id // Hydraulics Lab
                }
            };
            await Context.InstructorLabLink.AddRangeAsync(links);
            await Context.SaveChangesAsync();
        }
    }
} 

