using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class UniversitySeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Universities.AnyAsync())
        {
            var universities = new List<University>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة دمشق", En = "Damascus University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة حلب", En = "Aleppo University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة تشرين", En = "Tishreen University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة البعث", En = "Al-Baath University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة الفرات", En = "Al-Furat University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة حماة", En = "Hama University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة طرطوس", En = "Tartus University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة القلمون", En = "Al-Qalamoun University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة الوادي الدولية الخاصة", En = "Wadi International University" },
                    Type = new MultilingualText { Ar = "خاصة", En = "Private" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة الشام الخاصة", En = "Al-Sham Private University" },
                    Type = new MultilingualText { Ar = "خاصة", En = "Private" }
                }
            };

            await Context.Universities.AddRangeAsync(universities);
            await Context.SaveChangesAsync();
        }
    }
}
