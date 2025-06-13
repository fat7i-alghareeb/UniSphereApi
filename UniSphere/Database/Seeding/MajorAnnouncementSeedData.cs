using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class MajorAnnouncementSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.MajorAnnouncements.AnyAsync())
        {
            // Load majors with subjects eagerly loaded
            List<Major> majors = await Context.Majors.Include(m => m.Subjects).ToListAsync();

            if (majors.Count == 0)
                {
                  return;
                }

            var announcements = new List<MajorAnnouncement>();

            foreach (Major major in majors)
            {
                List<Subject> subjects = major.Subjects;
                if (subjects.Count == 0)
                {
                    // If no subjects, skip or create general announcements without subject reference
                    continue;
                }

                // Create 3 unique announcements per major
                for (int year = 1; year <= 3; year++)
                {
                    // Round robin selection of subjects for announcements
                    Subject  subject = subjects[(year - 1) % subjects.Count];

                    var title = new MultilingualText
                    {
                        Ar = $"إعلان هام لسنة {year} - {subject.Name.Ar}",
                        En = $"Important Announcement for Year {year} - {subject.Name.En}"
                    };

                    var content = new MultilingualText
                    {
                        Ar = $"يرجى ملاحظة أن نتائج منتصف الفصل في مادة {subject.Name.Ar} للسنة {year} ستصدر قريبًا. " +
                             $"الامتحان النصفي يشكل {subject.MidtermGrade}% من الدرجة النهائية، والامتحان النهائي يشكل {subject.FinalGrade}%. " +
                             "يرجى التحضير جيدًا.",

                        En = $"Please note that the midterm results for {subject.Name.En} for year {year} will be released soon. " +
                             $"The midterm exam accounts for {subject.MidtermGrade}% of the final grade, and the final exam accounts for {subject.FinalGrade}%. " +
                             "Please prepare accordingly."
                    };

                    announcements.Add(new MajorAnnouncement
                    {
                        Id = Guid.NewGuid(),
                        Title = title,
                        Content = content,
                        CreatedAt = DateTime.UtcNow,
                        Year = year,
                        SubjectId = subject.Id,
                        MajorId = major.Id
                    });
                }
            }

            await Context.MajorAnnouncements.AddRangeAsync(announcements);
            await Context.SaveChangesAsync();
        }
    }
}
