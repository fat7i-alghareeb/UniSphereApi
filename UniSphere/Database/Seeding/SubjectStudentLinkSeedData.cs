using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SubjectStudentLinkSeedData(ApplicationDbContext context) : SeedData(context)
{

    public override async Task SeedAsync()
    {
        if (!await Context.SubjectStudentLinks.AnyAsync())
        {
            List<Subject> subjects = await Context.Subjects.ToListAsync();
            List<StudentCredential> students = await Context.StudentCredentials.ToListAsync();
            if(subjects.Count == 0 || students.Count == 0)
            {
                return;
            }
            var subjectStudentLinks = new List<SubjectStudentLink>
            {
                new()
                {
                    SubjectId = subjects[0].Id,
                    StudentId = students[0].Id,
                    FacultyId = students[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[1].Id,
                    StudentId = students[0].Id,
                    FacultyId = students[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[2].Id,
                    StudentId = students[1].Id,
                    FacultyId = students[1].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                }
            };

            await Context.SubjectStudentLinks.AddRangeAsync(subjectStudentLinks);
            await Context.SaveChangesAsync();
        }
    }
} 