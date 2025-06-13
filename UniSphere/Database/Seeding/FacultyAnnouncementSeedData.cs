using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class FacultyAnnouncementSeedData : SeedData
{
    public FacultyAnnouncementSeedData(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task SeedAsync()
    {
        if (await Context.Set<FacultyAnnouncement>().AnyAsync())
        {
            return;
        }

        List<Faculty> faculty = await Context.Faculties.ToListAsync();
        List<FacultyAnnouncement> facultyAnnouncements = new()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = new MultilingualText
                {
                    Ar = "بدء التسجيل للفصل الدراسي الجديد",
                    En = "Registration Start for New Semester"
                },
                Content = new MultilingualText
                {
                    Ar = "نود إعلامكم أن التسجيل للفصل الدراسي الجديد سيبدأ في الأول من سبتمبر. يرجى مراجعة موقع الجامعة للحصول على المزيد من المعلومات.",
                    En = "We would like to inform you that registration for the new semester will begin on September 1st. Please check the university website for more information."
                },
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                FacultyId = faculty[0].Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = new MultilingualText
                {
                    Ar = "ورشة عمل في مجال الذكاء الاصطناعي",
                    En = "Workshop on Artificial Intelligence"
                },
                Content = new MultilingualText
                {
                    Ar = "سيتم تنظيم ورشة عمل حول الذكاء الاصطناعي في كلية الهندسة يوم الخميس القادم. جميع الطلاب مدعوون للمشاركة.",
                    En = "A workshop on Artificial Intelligence will be organized at the Faculty of Engineering next Thursday. All students are invited to participate."
                },
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                FacultyId = faculty[1].Id
            }
        };

        await Context.FacultyAnnouncements.AddRangeAsync(facultyAnnouncements);
        await Context.SaveChangesAsync();
    }
}
