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
                new() { SubjectId = subjects[0].Id, ProfessorId = professors[0].Id },
                new() { SubjectId = subjects[1].Id, ProfessorId = professors[1].Id },
                new() { SubjectId = subjects[2].Id, ProfessorId = professors[2].Id }
            };

            await Context.SubjectProfessorLinks.AddRangeAsync(subjectProfessorLinks);
            await Context.SaveChangesAsync();
        }
    }
} 