using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SubjectStudentLinkSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.SubjectStudentLinks.AnyAsync())
        {
            var firstStudentId = Guid.Parse("0c577686-efd9-40ab-b454-c5bbac8a4c95");
            var subjects = await Context.Subjects.OrderBy(s => s.Year).ToListAsync();
            if (subjects.Count == 0)
            {
                return;
            }
            var subjectStudentLinks = new List<SubjectStudentLink>
            {
                // Informatics Engineering Student Enrollments
                new()
                {
                    SubjectId = subjects[0].Id, // Web Programming
                    StudentId = firstStudentId,
                    AttemptNumber = 2,
                    IsCurrentlyEnrolled = false,
                    IsPassed =true,
                    MidtermGrade = 30,
                    FinalGrade = 40

                },
                new()
                {
                    SubjectId = subjects[1].Id, // Web Programming
                    StudentId = firstStudentId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = false,
                    IsPassed =true,
                    MidtermGrade = 30,
                    FinalGrade = 55
                },
                new()
                {
                    SubjectId = subjects[2].Id, // Web Programming
                    StudentId = firstStudentId,
                    AttemptNumber = 0,
                    IsCurrentlyEnrolled = false,
                    IsPassed =true,
                    MidtermGrade = 30,
                    FinalGrade = 55
                },
                new()
                {
                        SubjectId = subjects[3].Id, // Web Programming
                    StudentId = firstStudentId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true,
                    IsPassed =false,
                    MidtermGrade = 10,
                    FinalGrade = 10
                },
                new()
                {
                         SubjectId = subjects[4].Id, // Web Programming
                    StudentId = firstStudentId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true,
                    IsPassed =false,
                    // MidtermGrade = 10,
                    // FinalGrade = 10
                },

                new()
                {
                    SubjectId = subjects[5].Id, // Web Programming
                    StudentId = firstStudentId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true,
                    IsPassed =false,
                    // MidtermGrade = 30,
                    // FinalGrade = 40

                },
             
             
                // new()
                // {
                //     SubjectId = subjects[2].Id, // Advanced Programming
                //     StudentId = firstStudentId,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },
                // new()
                // {
                //     SubjectId = subjects[3].Id, // AI
                //     StudentId = firstStudentId,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },

                // // Computer Engineering Student Enrollments
                // new()
                // {
                //     SubjectId = subjects[4].Id, // Computer Architecture
                //     StudentId = students[1].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },
                // new()
                // {
                //     SubjectId = subjects[5].Id, // Computer Networks
                //     StudentId = students[1].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },

                // // Electrical Engineering Student Enrollments
                // new()
                // {
                //     SubjectId = subjects[6].Id, // Electric Circuits
                //     StudentId = students[4].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },
                // new()
                // {
                //     SubjectId = subjects[7].Id, // Electrical Machines
                //     StudentId = students[4].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },

                // // Civil Engineering Student Enrollments
                // new()
                // {
                //     SubjectId = subjects[8].Id, // Soil Mechanics
                //     StudentId = students[8].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },
                // new()
                // {
                //     SubjectId = subjects[9].Id, // Concrete Structure Design
                //     StudentId = students[8].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },

                // // Medical Student Enrollments
                // new()
                // {
                //     SubjectId = subjects[10].Id, // Anatomy
                //     StudentId = students[10].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // },
                // new()
                // {
                //     SubjectId = subjects[11].Id, // Physiology
                //     StudentId = students[10].Id,
                //     AttemptNumber = 1,
                //     IsCurrentlyEnrolled = true
                // }
            };

            await Context.SubjectStudentLinks.AddRangeAsync(subjectStudentLinks);
            await Context.SaveChangesAsync();
        }
    }
}
