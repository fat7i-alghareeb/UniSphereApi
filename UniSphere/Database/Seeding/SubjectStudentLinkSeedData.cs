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
                // Informatics Engineering Student Enrollments
                new()
                {
                    SubjectId = subjects[0].Id, // Web Programming
                    StudentId = students[0].Id,
                    FacultyId = students[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[1].Id, // Databases
                    StudentId = students[0].Id,
                    FacultyId = students[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[2].Id, // Advanced Programming
                    StudentId = students[0].Id,
                    FacultyId = students[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[3].Id, // AI
                    StudentId = students[0].Id,
                    FacultyId = students[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },

                // Computer Engineering Student Enrollments
                new()
                {
                    SubjectId = subjects[4].Id, // Computer Architecture
                    StudentId = students[1].Id,
                    FacultyId = students[1].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[5].Id, // Computer Networks
                    StudentId = students[1].Id,
                    FacultyId = students[1].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },

                // Electrical Engineering Student Enrollments
                new()
                {
                    SubjectId = subjects[6].Id, // Electric Circuits
                    StudentId = students[4].Id,
                    FacultyId = students[4].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[7].Id, // Electrical Machines
                    StudentId = students[4].Id,
                    FacultyId = students[4].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },

                // Civil Engineering Student Enrollments
                new()
                {
                    SubjectId = subjects[8].Id, // Soil Mechanics
                    StudentId = students[8].Id,
                    FacultyId = students[8].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[9].Id, // Concrete Structure Design
                    StudentId = students[8].Id,
                    FacultyId = students[8].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },

                // Medical Student Enrollments
                new()
                {
                    SubjectId = subjects[10].Id, // Anatomy
                    StudentId = students[10].Id,
                    FacultyId = students[10].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[11].Id, // Physiology
                    StudentId = students[10].Id,
                    FacultyId = students[10].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                }
            };

            await Context.SubjectStudentLinks.AddRangeAsync(subjectStudentLinks);
            await Context.SaveChangesAsync();
        }
    }
} 