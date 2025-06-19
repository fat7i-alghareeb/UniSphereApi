using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class ProfessorSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Professors.AnyAsync())
        {
            var professors = new List<Professor>
            {
                // Informatics Engineering Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999991"),
                    FirstName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Gmail = "ahmed.mohammed@unisphere.com",
                    OneTimeCode = 1111,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذ في هندسة المعلوماتية", En = "Professor of Informatics Engineering" },
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999992"),
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Gmail = "sarah.ali@unisphere.com",
                    OneTimeCode = 2222,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذة في هندسة المعلوماتية", En = "Professor of Informatics Engineering" },
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999993"),
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    Gmail = "mohammed.khaled@unisphere.com",
                    OneTimeCode = 3333,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذ في هندسة المعلوماتية", En = "Professor of Informatics Engineering" },
                    Image = "https://via.placeholder.com/150"
                },

                // Electrical Engineering Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999994"),
                    FirstName = new MultilingualText { Ar = "علي", En = "Ali" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" },
                    Gmail = "ali.hussein@unisphere.com",
                    OneTimeCode = 4444,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذ في الهندسة الكهربائية", En = "Professor of Electrical Engineering" },
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999995"),
                    FirstName = new MultilingualText { Ar = "فاطمة", En = "Fatima" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Gmail = "fatima.ahmed@unisphere.com",
                    OneTimeCode = 5555,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذة في الهندسة الكهربائية", En = "Professor of Electrical Engineering" },
                    Image = "https://via.placeholder.com/150"
                },

                // Mechanical Engineering Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999996"),
                    FirstName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Gmail = "khaled.mohammed@unisphere.com",
                    OneTimeCode = 6666,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذ في الهندسة الميكانيكية", En = "Professor of Mechanical Engineering" },
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999997"),
                    FirstName = new MultilingualText { Ar = "نور", En = "Nour" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Gmail = "nour.ali@unisphere.com",
                    OneTimeCode = 7777,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذة في الهندسة الميكانيكية", En = "Professor of Mechanical Engineering" },
                    Image = "https://via.placeholder.com/150"
                },

                // Civil Engineering Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999998"),
                    FirstName = new MultilingualText { Ar = "عمر", En = "Omar" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" },
                    Gmail = "omar.hussein@unisphere.com",
                    OneTimeCode = 8888,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذ في الهندسة المدنية", En = "Professor of Civil Engineering" },
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    FirstName = new MultilingualText { Ar = "ليلى", En = "Layla" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Gmail = "layla.ahmed@unisphere.com",
                    OneTimeCode = 9999,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذة في الهندسة المدنية", En = "Professor of Civil Engineering" },
                    Image = "https://via.placeholder.com/150"
                },

                // Medical Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-99999999999a"),
                    FirstName = new MultilingualText { Ar = "د. محمد", En = "Dr. Mohammed" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Gmail = "dr.mohammed.ali@unisphere.com",
                    OneTimeCode = 1010,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذ في الطب", En = "Professor of Medicine" },
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-99999999999b"),
                    FirstName = new MultilingualText { Ar = "د. سارة", En = "Dr. Sarah" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    Gmail = "dr.sarah.khaled@unisphere.com",
                    OneTimeCode = 2020,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    Bio = new MultilingualText { Ar = "أستاذة في الطب", En = "Professor of Medicine" },
                    Image = "https://via.placeholder.com/150"
                }
            };

            await Context.Professors.AddRangeAsync(professors);
            await Context.SaveChangesAsync();
        }
    }
} 