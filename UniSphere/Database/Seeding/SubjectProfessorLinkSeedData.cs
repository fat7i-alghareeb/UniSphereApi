using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SubjectProfessorLinkSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.SubjectProfessorLinks.AnyAsync())
        {
            List<Subject> subjects = await Context.Subjects.ToListAsync();
            List<Professor> professors = await Context.Professors.ToListAsync();
            if(subjects.Count == 0 || professors.Count == 0)
            {
                return;
            }
            var subjectProfessorLinks = new List<SubjectProfessorLink>
            {
                // Informatics Engineering Subjects
                new() 
                { 
                    SubjectId = subjects[0].Id, // Web Programming
                    ProfessorId = professors[0].Id 
                },
                new() 
                { 
                    SubjectId = subjects[1].Id, // Databases
                    ProfessorId = professors[1].Id 
                },
                new() 
                { 
                    SubjectId = subjects[2].Id, // Advanced Programming
                    ProfessorId = professors[2].Id 
                },
                new() 
                { 
                    SubjectId = subjects[3].Id, // AI
                    ProfessorId = professors[3].Id 
                },

                // Computer Engineering Subjects
                new() 
                { 
                    SubjectId = subjects[4].Id, // Computer Architecture
                    ProfessorId = professors[4].Id 
                },
                new() 
                { 
                    SubjectId = subjects[5].Id, // Computer Networks
                    ProfessorId = professors[5].Id 
                },

                // Electrical Engineering Subjects
                new() 
                { 
                    SubjectId = subjects[6].Id, // Electric Circuits
                    ProfessorId = professors[6].Id 
                },
                new() 
                { 
                    SubjectId = subjects[7].Id, // Electrical Machines
                    ProfessorId = professors[7].Id 
                },

                // Civil Engineering Subjects
                new() 
                { 
                    SubjectId = subjects[8].Id, // Soil Mechanics
                    ProfessorId = professors[8].Id 
                },
                new() 
                { 
                    SubjectId = subjects[9].Id, // Concrete Structure Design
                    ProfessorId = professors[9].Id 
                },

                // Medical Subjects
                new() 
                { 
                    SubjectId = subjects[10].Id, // Anatomy
                    ProfessorId = professors[10].Id 
                },
                new() 
                { 
                    SubjectId = subjects[11].Id, // Physiology
                    ProfessorId = professors[11].Id 
                }
            };

            await Context.SubjectProfessorLinks.AddRangeAsync(subjectProfessorLinks);
            await Context.SaveChangesAsync();
        }
    }
} 