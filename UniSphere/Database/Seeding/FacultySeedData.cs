using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class FacultySeedData(ApplicationDbContext context, ILogger<FacultySeedData> logger) : SeedData(context)
{
    private readonly ILogger<FacultySeedData> _logger = logger;

    public override async Task SeedAsync()
    {
        if (!await Context.Faculties.AnyAsync())
        {
            List<University> universities = await Context.Universities.ToListAsync();
            _logger.LogInformation("Found {UniversityCount} universities in the database", universities.Count);
            
            if(universities.Count == 0)
            {
                _logger.LogWarning("No universities found in database. Skipping faculty seeding.");
                return;
            }

            _logger.LogInformation("First university ID: {FirstUniversityId}", universities[0].Id);
            _logger.LogInformation("Second university ID: {SecondUniversityId}", universities[1].Id);
            _logger.LogInformation("Third university ID: {ThirdUniversityId}", universities[2].Id);

            var faculties = new List<Faculty>
            {
                // Damascus University Faculties
                new()
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعلوماتية", En = "Faculty of Informatics Engineering" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الميكانيكية والكهربائية", En = "Faculty of Mechanical and Electrical Engineering" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المدنية", En = "Faculty of Civil Engineering" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعمارية", En = "Faculty of Architecture" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الطب", En = "Faculty of Medicine" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية طب الأسنان", En = "Faculty of Dentistry" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الصيدلة", En = "Faculty of Pharmacy" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية العلوم", En = "Faculty of Science" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الاقتصاد", En = "Faculty of Economics" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الحقوق", En = "Faculty of Law" },
                    UniversityId = universities[0].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },

                // Aleppo University Faculties
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعلوماتية", En = "Faculty of Informatics Engineering" },
                    UniversityId = universities[1].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الميكانيكية", En = "Faculty of Mechanical Engineering" },
                    UniversityId = universities[1].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الكهربائية", En = "Faculty of Electrical Engineering" },
                    UniversityId = universities[1].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الطب", En = "Faculty of Medicine" },
                    UniversityId = universities[1].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية العلوم", En = "Faculty of Science" },
                    UniversityId = universities[1].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },

                // Tishreen University Faculties
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الميكانيكية والكهربائية", En = "Faculty of Mechanical and Electrical Engineering" },
                    UniversityId = universities[2].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المدنية", En = "Faculty of Civil Engineering" },
                    UniversityId = universities[2].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعمارية", En = "Faculty of Architecture" },
                    UniversityId = universities[2].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الطب", En = "Faculty of Medicine" },
                    UniversityId = universities[2].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية العلوم", En = "Faculty of Science" },
                    UniversityId = universities[2].Id,
                    DaysToTheFinale = DateTime.UtcNow.AddDays(30)
                }
            };

            _logger.LogInformation("Adding {FacultyCount} faculties to the database", faculties.Count);
            await Context.Faculties.AddRangeAsync(faculties);
            await Context.SaveChangesAsync();
            _logger.LogInformation("Faculty seeding completed successfully");
        }
        else
        {
            _logger.LogInformation("Faculties already exist in database. Skipping faculty seeding.");
        }
    }
}
