using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SubjectSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Subjects.AnyAsync())
        {
            List<Major> majors = await Context.Majors.ToListAsync();
            if(majors.Count == 0)
            {
                return;
            }
            var subjects = new List<Subject>
            {
                // Informatics Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "برمجة الويب", En = "Web Programming" },
                    Description = new MultilingualText { Ar = "مقدمة في تطوير تطبيقات الويب", En = "Introduction to Web Application Development" },
                    MajorId = majors[0].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "قواعد البيانات", En = "Databases" },
                    Description = new MultilingualText { Ar = "مقدمة في قواعد البيانات وإدارتها", En = "Introduction to Databases and Management" },
                    MajorId = majors[0].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "برمجة متقدمة", En = "Advanced Programming" },
                    Description = new MultilingualText { Ar = "مفاهيم البرمجة المتقدمة وأنماط التصميم", En = "Advanced Programming Concepts and Design Patterns" },
                    MajorId = majors[0].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الذكاء الاصطناعي", En = "Artificial Intelligence" },
                    Description = new MultilingualText { Ar = "مقدمة في الذكاء الاصطناعي وتعلم الآلة", En = "Introduction to AI and Machine Learning" },
                    MajorId = majors[0].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Computer Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "معمارية الحاسوب", En = "Computer Architecture" },
                    Description = new MultilingualText { Ar = "دراسة بنية وتصميم المعالجات", En = "Study of Processor Design and Architecture" },
                    MajorId = majors[1].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "شبكات الحاسوب", En = "Computer Networks" },
                    Description = new MultilingualText { Ar = "مبادئ وأساسيات شبكات الحاسوب", En = "Principles and Fundamentals of Computer Networks" },
                    MajorId = majors[1].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Electrical Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الدوائر الكهربائية", En = "Electric Circuits" },
                    Description = new MultilingualText { Ar = "تحليل وتصميم الدوائر الكهربائية", En = "Analysis and Design of Electric Circuits" },
                    MajorId = majors[4].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الآلات الكهربائية", En = "Electrical Machines" },
                    Description = new MultilingualText { Ar = "دراسة المحركات والمولدات الكهربائية", En = "Study of Electric Motors and Generators" },
                    MajorId = majors[4].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Civil Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "ميكانيكا التربة", En = "Soil Mechanics" },
                    Description = new MultilingualText { Ar = "دراسة خصائص وسلوك التربة", En = "Study of Soil Properties and Behavior" },
                    MajorId = majors[8].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "تصميم المنشآت الخرسانية", En = "Concrete Structure Design" },
                    Description = new MultilingualText { Ar = "تصميم وتحليل المنشآت الخرسانية", En = "Design and Analysis of Concrete Structures" },
                    MajorId = majors[8].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Medical Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "علم التشريح", En = "Anatomy" },
                    Description = new MultilingualText { Ar = "دراسة تشريح جسم الإنسان", En = "Study of Human Body Anatomy" },
                    MajorId = majors[12].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = true,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "علم وظائف الأعضاء", En = "Physiology" },
                    Description = new MultilingualText { Ar = "دراسة وظائف أعضاء جسم الإنسان", En = "Study of Human Body Physiology" },
                    MajorId = majors[12].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = true,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                }
            };

            await Context.Subjects.AddRangeAsync(subjects);
            await Context.SaveChangesAsync();
        }
    }
} 
