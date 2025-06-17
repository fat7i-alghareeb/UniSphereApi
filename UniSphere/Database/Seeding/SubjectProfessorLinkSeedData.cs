using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SubjectProfessorLinkSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.SubjectProfessorLinks.AnyAsync())
        {
            var subjects = await Context.Subjects.OrderBy(s => s.Year).ToListAsync();
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
                    ProfessorId = professors[0].Id // Ahmed Mohammed
                },
                new() 
                { 
                    SubjectId = subjects[1].Id, // Databases
                    ProfessorId = professors[1].Id // Sarah Ali
                },
                new() 
                { 
                    SubjectId = subjects[2].Id, // Advanced Programming
                    ProfessorId = professors[2].Id // Mohammed Khaled
                },
                new() 
                { 
                    SubjectId = subjects[3].Id, // AI
                    ProfessorId = professors[0].Id // Ahmed Mohammed
                },

                // Computer Engineering Subjects
                new() 
                { 
                    SubjectId = subjects[4].Id, // Computer Architecture
                    ProfessorId = professors[2].Id // Mohammed Khaled
                },
                new() 
                { 
                    SubjectId = subjects[5].Id, // Computer Networks
                    ProfessorId = professors[1].Id // Sarah Ali
                },

                // Electrical Engineering Subjects
                new() 
                { 
                    SubjectId = subjects[6].Id, // Electric Circuits
                    ProfessorId = professors[3].Id // Ali Hussein
                },
                new() 
                { 
                    SubjectId = subjects[7].Id, // Electrical Machines
                    ProfessorId = professors[4].Id // Fatima Ahmed
                },

                // Civil Engineering Subjects
                new() 
                { 
                    SubjectId = subjects[8].Id, // Soil Mechanics
                    ProfessorId = professors[7].Id // Omar Hussein
                },
                new() 
                { 
                    SubjectId = subjects[9].Id, // Concrete Structure Design
                    ProfessorId = professors[8].Id // Layla Ahmed
                },

                // Medical Subjects
                new() 
                { 
                    SubjectId = subjects[10].Id, // Anatomy
                    ProfessorId = professors[9].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[11].Id, // Physiology
                    ProfessorId = professors[9].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[12].Id, // Physiology
                    ProfessorId = professors[2].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[13].Id, // Physiology
                    ProfessorId = professors[6].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[14].Id, // Physiology
                    ProfessorId = professors[2].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[15].Id, // Physiology
                    ProfessorId = professors[5].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[16].Id, // Physiology
                    ProfessorId = professors[1].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[17].Id, // Physiology
                    ProfessorId = professors[7].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[18].Id, // Physiology
                    ProfessorId = professors[8].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[19].Id, // Physiology
                    ProfessorId = professors[1].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[20].Id, // Physiology
                    ProfessorId = professors[0].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[21].Id, // Physiology
                    ProfessorId = professors[2].Id // Dr. Mohammed Ali
                },
                new() 
                { 
                    SubjectId = subjects[22].Id, // Physiology
                    ProfessorId = professors[1].Id // Dr. Mohammed Ali
                },
            };

            await Context.SubjectProfessorLinks.AddRangeAsync(subjectProfessorLinks);
            await Context.SaveChangesAsync();
        }
    }
} 
