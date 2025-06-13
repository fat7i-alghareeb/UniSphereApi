using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class MajorAnnouncementSeedData : SeedData
{
    public MajorAnnouncementSeedData(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task SeedAsync()
    {
        if (await Context.Set<MajorAnnouncement>().AnyAsync())
        {
            return;
        }

        List<Major> majorId = await Context.Majors.ToListAsync();
        List<Subject> subjectId = await Context.Subjects.ToListAsync();

        var majorAnnouncements = new List<MajorAnnouncement>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = new MultilingualText
                {
                    Ar = "موعد امتحان البرمجة المتقدمة",
                    En = "Advanced Programming Exam Schedule"
                },
                Content = new MultilingualText
                {
                    Ar = "سيتم عقد امتحان البرمجة المتقدمة يوم الثلاثاء القادم في القاعة رقم 302. يرجى الحضور قبل الموعد بـ 15 دقيقة.",
                    En = "The Advanced Programming exam will be held next Tuesday in Room 302. Please arrive 15 minutes before the scheduled time."
                },
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                MajorId = majorId[0].Id,
                SubjectId = subjectId[0].Id
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = new MultilingualText
                {
                    Ar = "تغيير موعد محاضرة قواعد البيانات",
                    En = "Database Lecture Time Change"
                },
                Content = new MultilingualText
                {
                    Ar = "تم تغيير موعد محاضرة قواعد البيانات من الساعة 10 صباحاً إلى الساعة 2 ظهراً. يرجى مراعاة هذا التغيير.",
                    En = "The Database lecture time has been changed from 10 AM to 2 PM. Please take note of this change."
                },
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                MajorId = majorId[0].Id,
                SubjectId = subjectId[0].Id
            }
        };

        await Context.Set<MajorAnnouncement>().AddRangeAsync(majorAnnouncements);
        await Context.SaveChangesAsync();
    }
}
