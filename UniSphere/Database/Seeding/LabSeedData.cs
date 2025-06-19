using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class LabSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Labs.AnyAsync())
        {
            var labs = new List<Lab>
            {
                // Informatics Engineering Labs
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333331"),
                    Name = new MultilingualText { Ar = "مخبر البرمجة المتقدمة", En = "Advanced Programming Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم البرمجة المتقدمة وتطوير البرمجيات", En = "Lab for teaching advanced programming and software development" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333332"),
                    Name = new MultilingualText { Ar = "مخبر الشبكات", En = "Networks Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الشبكات وتصميمها", En = "Lab for teaching networks and network design" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = new MultilingualText { Ar = "مخبر الذكاء الاصطناعي", En = "Artificial Intelligence Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الذكاء الاصطناعي وتطبيقاته", En = "Lab for teaching artificial intelligence and its applications" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333334"),
                    Name = new MultilingualText { Ar = "مخبر قواعد البيانات", En = "Database Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم قواعد البيانات وإدارتها", En = "Lab for teaching databases and database management" }
                },

                // Electrical Engineering Labs
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333335"),
                    Name = new MultilingualText { Ar = "مخبر الدوائر الكهربائية", En = "Electrical Circuits Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم وتحليل الدوائر الكهربائية", En = "Lab for teaching and analyzing electrical circuits" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333336"),
                    Name = new MultilingualText { Ar = "مخبر الإلكترونيات", En = "Electronics Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الإلكترونيات وتطبيقاتها", En = "Lab for teaching electronics and its applications" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333337"),
                    Name = new MultilingualText { Ar = "مخبر الاتصالات", En = "Communications Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم نظم الاتصالات", En = "Lab for teaching communication systems" }
                },

                // Mechanical Engineering Labs
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333338"),
                    Name = new MultilingualText { Ar = "مخبر الميكانيك", En = "Mechanics Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الميكانيك وتطبيقاته", En = "Lab for teaching mechanics and its applications" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333339"),
                    Name = new MultilingualText { Ar = "مخبر التصميم الميكانيكي", En = "Mechanical Design Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم التصميم الميكانيكي", En = "Lab for teaching mechanical design" }
                },

                // Civil Engineering Labs
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-33333333333a"),
                    Name = new MultilingualText { Ar = "مخبر المواد الإنشائية", En = "Construction Materials Lab" },
                    Description = new MultilingualText { Ar = "مخبر لاختبار المواد الإنشائية", En = "Lab for testing construction materials" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-33333333333b"),
                    Name = new MultilingualText { Ar = "مخبر الهيدروليك", En = "Hydraulics Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الهيدروليك وتطبيقاته", En = "Lab for teaching hydraulics and its applications" }
                },

                // Science Labs
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-33333333333c"),
                    Name = new MultilingualText { Ar = "مخبر الفيزياء", En = "Physics Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الفيزياء وتجاربها", En = "Lab for teaching physics and experiments" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-33333333333d"),
                    Name = new MultilingualText { Ar = "مخبر الكيمياء", En = "Chemistry Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الكيمياء وتجاربها", En = "Lab for teaching chemistry and experiments" }
                },
                new()
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-33333333333e"),
                    Name = new MultilingualText { Ar = "مخبر الأحياء", En = "Biology Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الأحياء وتجاربها", En = "Lab for teaching biology and experiments" }
                }
            };

            await Context.Labs.AddRangeAsync(labs);
            await Context.SaveChangesAsync();
        }
    }
} 
