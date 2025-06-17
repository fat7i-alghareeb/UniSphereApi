using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SubjectSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Subjects.AnyAsync())
        {
            var firstMajorGuid = Guid.TryParse("09da2b33-d994-4a4f-9271-5056165a7146", out var guid1)
                ? guid1
                : Guid.NewGuid();
            var majors = await Context.Majors.ToListAsync();
            if(majors.Count == 0)
            {
                return;
            }
            List<Subject> subjects = new List<Subject>
            {
                // Year 1 - Software Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 1,
                    Name = new MultilingualText { Ar = "برمجة الويب", En = "Web Programming" },
                    Description = new MultilingualText { Ar = "مقدمة في تطوير تطبيقات الويب", En = "Introduction to Web Application Development" },
                    MajorId = firstMajorGuid,
                    Semester = 1,
                    IsLabRequired = true,       
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 1,
                    Name = new MultilingualText { Ar = "قواعد البيانات", En = "Databases" },
                    Description = new MultilingualText { Ar = "مقدمة في قواعد البيانات وإدارتها", En = "Introduction to Databases and Management" },
                    MajorId = firstMajorGuid,
                    Semester = 1,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,

                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 1,
                    Name = new MultilingualText { Ar = "برمجة متقدمة", En = "Advanced Programming" },
                    Description = new MultilingualText { Ar = "مفاهيم البرمجة المتقدمة وأنماط التصميم", En = "Advanced Programming Concepts and Design Patterns" },
                    MajorId = firstMajorGuid,
                    Semester = 1,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 1,
                    Name = new MultilingualText { Ar = "الرياضيات", En = "Mathematics" },
                    Description = new MultilingualText { Ar = "الرياضيات الأساسية للهندسة", En = "Basic Mathematics for Engineering" },
                    MajorId = firstMajorGuid,
                    Semester = 1,
                    IsLabRequired = false,
                    IsMultipleChoice = true,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 1,
                    Name = new MultilingualText { Ar = "الفيزياء", En = "Physics" },
                    Description = new MultilingualText { Ar = "مبادئ الفيزياء الأساسية", En = "Basic Physics Principles" },
                    MajorId = firstMajorGuid,
                    Semester = 2,
                    IsLabRequired = true,
                    IsMultipleChoice = true,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Year 2 - Software Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 2,
                    Name = new MultilingualText { Ar = "هندسة البرمجيات", En = "Software Engineering" },
                    Description = new MultilingualText { Ar = "مبادئ وأساليب هندسة البرمجيات", En = "Software Engineering Principles and Methods" },
                    MajorId = firstMajorGuid,
                    Semester = 1,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 2,
                    Name = new MultilingualText { Ar = "تحليل وتصميم الخوارزميات", En = "Algorithm Analysis and Design" },
                    Description = new MultilingualText { Ar = "تحليل وتصميم الخوارزميات المتقدمة", En = "Advanced Algorithm Analysis and Design" },
                    MajorId = firstMajorGuid,
                    Semester = 1,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 2,
                    Name = new MultilingualText { Ar = "برمجة تطبيقات الموبايل", En = "Mobile Application Development" },
                    Description = new MultilingualText { Ar = "تطوير تطبيقات الهواتف الذكية", En = "Smartphone Application Development" },
                    MajorId = firstMajorGuid,
                    Semester = 2,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 2,
                    Name = new MultilingualText { Ar = "أمن المعلومات", En = "Information Security" },
                    Description = new MultilingualText { Ar = "مبادئ وأساسيات أمن المعلومات", En = "Information Security Principles and Fundamentals" },
                    MajorId = firstMajorGuid,
                    Semester = 2,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 2,
                    Name = new MultilingualText { Ar = "برمجة الشبكات", En = "Network Programming" },
                    Description = new MultilingualText { Ar = "برمجة تطبيقات الشبكات", En = "Network Applications Programming" },
                    MajorId = firstMajorGuid,
                    Semester = 2,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Year 3 - Software Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 3,
                    Name = new MultilingualText { Ar = "تطوير تطبيقات الويب المتقدمة", En = "Advanced Web Development" },
                    Description = new MultilingualText { Ar = "تطوير تطبيقات الويب المتقدمة والأطر البرمجية", En = "Advanced Web Development and Frameworks" },
                    MajorId = firstMajorGuid,
                    Semester = 3,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 3,
                    Name = new MultilingualText { Ar = "قواعد البيانات المتقدمة", En = "Advanced Databases" },
                    Description = new MultilingualText { Ar = "إدارة وتصميم قواعد البيانات المتقدمة", En = "Advanced Database Management and Design" },
                    MajorId = firstMajorGuid,
                    Semester = 3,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 3,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات الموزعة", En = "Distributed Software Development" },
                    Description = new MultilingualText { Ar = "تصميم وتطوير البرمجيات الموزعة", En = "Design and Development of Distributed Software" },
                    MajorId = firstMajorGuid,
                    Semester = 3,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 3,
                    Name = new MultilingualText { Ar = "الذكاء الاصطناعي", En = "Artificial Intelligence" },
                    Description = new MultilingualText { Ar = "مقدمة في الذكاء الاصطناعي وتعلم الآلة", En = "Introduction to AI and Machine Learning" },
                    MajorId = firstMajorGuid,
                    Semester = 3,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 3,
                    Name = new MultilingualText { Ar = "تطوير واجهات المستخدم", En = "User Interface Development" },
                    Description = new MultilingualText { Ar = "تصميم وتطوير واجهات المستخدم", En = "User Interface Design and Development" },
                    MajorId = firstMajorGuid,
                    Semester = 3,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Year 4 - Software Engineering Subjects
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 4,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات المتنقلة المتقدم", En = "Advanced Mobile Development" },
                    Description = new MultilingualText { Ar = "تطوير تطبيقات الموبايل المتقدمة", En = "Advanced Mobile Application Development" },
                    MajorId = firstMajorGuid,
                    Semester = 4,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 4,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات المضمنة", En = "Embedded Software Development" },
                    Description = new MultilingualText { Ar = "تطوير البرمجيات للأنظمة المضمنة", En = "Software Development for Embedded Systems" },
                    MajorId = firstMajorGuid,
                    Semester = 4,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 4,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات السحابية", En = "Cloud Software Development" },
                    Description = new MultilingualText { Ar = "تطوير تطبيقات الحوسبة السحابية", En = "Cloud Computing Application Development" },
                    MajorId = firstMajorGuid,
                    Semester = 4,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 4,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات المالية", En = "Financial Software Development" },
                    Description = new MultilingualText { Ar = "تطوير تطبيقات البرمجيات المالية", En = "Financial Software Application Development" },
                    MajorId = firstMajorGuid,
                    Semester = 4,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 4,
                    Name = new MultilingualText { Ar = "مشروع التخرج", En = "Graduation Project" },
                    Description = new MultilingualText { Ar = "مشروع التخرج في هندسة البرمجيات", En = "Software Engineering Graduation Project" },
                    MajorId = firstMajorGuid,
                    Semester = 4,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Year 5 - Software Engineering Subjects (Optional/Elective)
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 5,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات المفتوحة", En = "Open Source Development" },
                    Description = new MultilingualText { Ar = "تطوير البرمجيات المفتوحة المصدر", En = "Open Source Software Development" },
                    MajorId = firstMajorGuid,
                    Semester = 5,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 5,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات للألعاب", En = "Game Development" },
                    Description = new MultilingualText { Ar = "تطوير برمجيات الألعاب", En = "Game Software Development" },
                    MajorId = firstMajorGuid,
                    Semester = 5,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Year = 5,
                    Name = new MultilingualText { Ar = "تطوير البرمجيات للأجهزة الذكية", En = "Smart Device Development" },
                    Description = new MultilingualText { Ar = "تطوير برمجيات الأجهزة الذكية", En = "Smart Device Software Development" },
                    MajorId = firstMajorGuid,
                    Semester = 5,
                        IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },

                // Keep existing subjects for other majors
                // ... existing code ...
            };
          
            await Context.Subjects.AddRangeAsync(subjects);
            await Context.SaveChangesAsync();
        }
    }
} 
