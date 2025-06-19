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
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" }
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999992"),
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" }
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999993"),
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" }
                },

                // Electrical Engineering Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999994"),
                    FirstName = new MultilingualText { Ar = "علي", En = "Ali" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" }
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999995"),
                    FirstName = new MultilingualText { Ar = "فاطمة", En = "Fatima" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" }
                },

                // Mechanical Engineering Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999996"),
                    FirstName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" }
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999997"),
                    FirstName = new MultilingualText { Ar = "نور", En = "Nour" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" }
                },

                // Civil Engineering Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999998"),
                    FirstName = new MultilingualText { Ar = "عمر", En = "Omar" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" }
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    FirstName = new MultilingualText { Ar = "ليلى", En = "Layla" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" }
                },

                // Medical Professors
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-99999999999a"),
                    FirstName = new MultilingualText { Ar = "د. محمد", En = "Dr. Mohammed" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" }
                },
                new()
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-99999999999b"),
                    FirstName = new MultilingualText { Ar = "د. سارة", En = "Dr. Sarah" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" }
                }
            };

            await Context.Professors.AddRangeAsync(professors);
            await Context.SaveChangesAsync();
        }
    }
} 