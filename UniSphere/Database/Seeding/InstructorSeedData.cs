using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class InstructorSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Instructors.AnyAsync())
        {
            var instructors = new List<Instructor>
            {
                // Informatics Engineering Instructors
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "رامي", En = "Rami" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    FatherName = new MultilingualText { Ar = "سامي", En = "Sami" },
                    Email = "rami.khaled@example.com",
                    Password = "password1"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "دينا", En = "Dina" },
                    LastName = new MultilingualText { Ar = "سعيد", En = "Saeed" },
                    FatherName = new MultilingualText { Ar = "فارس", En = "Fares" },
                    Email = "dina.saeed@example.com",
                    Password = "password2"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "عمر", En = "Omar" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Email = "omar.mohammed@example.com",
                    Password = "password3"
                },

                // Electrical Engineering Instructors
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    FatherName = new MultilingualText { Ar = "حسين", En = "Hussein" },
                    Email = "mohammed.ali@example.com",
                    Password = "password4"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    FatherName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    Email = "sarah.ahmed@example.com",
                    Password = "password5"
                },

                // Mechanical Engineering Instructors
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "علي", En = "Ali" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Email = "ali.hussein@example.com",
                    Password = "password6"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "فاطمة", En = "Fatima" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Email = "fatima.mohammed@example.com",
                    Password = "password7"
                },

                // Civil Engineering Instructors
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Email = "khaled.ali@example.com",
                    Password = "password8"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "نور", En = "Nour" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Email = "nour.ahmed@example.com",
                    Password = "password9"
                }
            };

            await Context.Instructors.AddRangeAsync(instructors);
            await Context.SaveChangesAsync();
        }
    }
} 