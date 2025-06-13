using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class FacultyAnnouncementSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.FacultyAnnouncements.AnyAsync())
        {
            var faculties = await Context.Faculties
                .Where(f =>
                    f.Name.En == "Faculty of Informatics Engineering" ||
                    f.Name.En == "Faculty of Mechanical and Electrical Engineering" ||
                    f.Name.En == "Faculty of Civil Engineering" ||
                    f.Name.En == "Faculty of Architecture" ||
                    f.Name.En == "Faculty of Medicine" ||
                    f.Name.En == "Faculty of Dentistry" ||
                    f.Name.En == "Faculty of Pharmacy" ||
                    f.Name.En == "Faculty of Science" ||
                    f.Name.En == "Faculty of Economics")
                .ToListAsync();

            var announcements = new List<FacultyAnnouncement>();

            foreach (var faculty in faculties)
            {
                List<FacultyAnnouncement> facultyAnnouncements = faculty.Name.En switch
                {
                    "Faculty of Informatics Engineering" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("تنويه تقني", "Tech Notice"),
                            Content = new("تم تحديث نظام تسجيل المقررات.", "The course registration system has been updated."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("إعلان ورشة برمجة", "Programming Workshop"),
                            Content = new("ندعوكم لحضور ورشة عمل حول تطوير تطبيقات الويب.", "Join us for a web app development workshop."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("أمن المعلومات", "Cybersecurity Update"),
                            Content = new("يرجى تغيير كلمات المرور الخاصة بحسابات الطلاب.", "Please update your student account passwords."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Mechanical and Electrical Engineering" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("صيانة مخابر", "Lab Maintenance"),
                            Content = new("المخابر ستكون مغلقة للصيانة يوم الأحد.", "Labs will be closed for maintenance on Sunday."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("ندوة محركات", "Engines Seminar"),
                            Content = new("محاضرة خاصة بمحركات الاحتراق الداخلي يوم الثلاثاء.", "Internal combustion engines lecture on Tuesday."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("ورشة طاقة متجددة", "Renewable Energy Workshop"),
                            Content = new("دعوة لحضور ورشة الطاقة المتجددة.", "Invitation to attend renewable energy workshop."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Civil Engineering" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("إغلاق مؤقت", "Temporary Closure"),
                            Content = new("مبنى الكلية سيُغلق مؤقتًا بسبب أعمال ترميم.", "The faculty building will be temporarily closed for renovation."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("رحلة ميدانية", "Field Trip"),
                            Content = new("تنظم الكلية رحلة إلى موقع بناء في ضواحي دمشق.", "A field trip to a construction site near Damascus is planned."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("مؤتمر هندسي", "Engineering Conference"),
                            Content = new("دعوة للمشاركة في مؤتمر الهندسة المدنية السنوي.", "Invitation to the annual civil engineering conference."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Architecture" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("مسابقة تصميم", "Design Competition"),
                            Content = new("إطلاق مسابقة لأفضل تصميم داخلي.", "Launching a competition for best interior design."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("عرض مشاريع", "Project Exhibition"),
                            Content = new("عرض مشاريع السنة النهائية الأسبوع المقبل.", "Final year projects exhibition next week."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("محاضرة ضيف", "Guest Lecture"),
                            Content = new("محاضرة معمارية من ضيف دولي يوم الأربعاء.", "Architecture talk by an international guest on Wednesday."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Medicine" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("دورة إسعافات", "First Aid Course"),
                            Content = new("تبدأ دورة الإسعافات الأولية يوم الإثنين.", "First aid course starts on Monday."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("مؤتمر علمي", "Medical Conference"),
                            Content = new("مؤتمر حول التطورات الحديثة في الطب يوم الجمعة.", "Conference on modern medical advances on Friday."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("سحب دم تطوعي", "Blood Donation"),
                            Content = new("ندعوكم للتبرع بالدم في بهو الكلية.", "Please donate blood in the faculty hall."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Dentistry" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("مخبر جديد", "New Lab Opening"),
                            Content = new("افتتاح مخبر تشخيص جديد في الطابق الثاني.", "New diagnostic lab opened on the second floor."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("دورة تعقيم", "Sterilization Training"),
                            Content = new("ورشة حول إجراءات التعقيم الحديثة.", "Workshop on modern sterilization procedures."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("إعلانات هامة", "Important Notices"),
                            Content = new("تحديث حول استخدام المعدات السنية.", "Update regarding dental equipment usage."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Pharmacy" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("محاضرة دوائية", "Pharmacology Lecture"),
                            Content = new("محاضرة حول الأدوية الجديدة في السوق.", "Lecture on new market drugs."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("تدريب صيدلي", "Pharmacy Training"),
                            Content = new("تدريب عملي في صيدليات الجامعة.", "Practical training in campus pharmacies."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("إعلان مختبرات", "Lab Notice"),
                            Content = new("يرجى إحضار المعاطف البيضاء لمختبر الغد.", "Please bring lab coats for tomorrow's session."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Science" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("أسبوع العلوم", "Science Week"),
                            Content = new("فعاليات أسبوع العلوم تبدأ الأحد.", "Science week activities start Sunday."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("ندوة كيميائية", "Chemistry Seminar"),
                            Content = new("ندوة خاصة حول المواد الكيميائية النانوية.", "Special seminar on nano-chemical materials."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("مشروع أبحاث", "Research Projects"),
                            Content = new("يرجى تقديم مقترحات مشاريع البحث.", "Please submit your research project proposals."),
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Economics" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("تحليل سوق", "Market Analysis"),
                            Content = new("ورشة حول تحليل السوق المالية المحلية.", "Workshop on analyzing the local financial market."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("مراجعة اختبارات", "Exam Review"),
                            Content = new("جلسة مراجعة لاختبار الاقتصاد الكلي.", "Macroeconomics exam review session."),
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new("فرص تدريب", "Internship Offers"),
                            Content = new("فرص تدريبية في بنوك محلية متاحة الآن.", "Internship opportunities now available in local banks."),
                            FacultyId = faculty.Id
                        }
                    },

                    _ => new()
                };

                announcements.AddRange(facultyAnnouncements);
            }

            await Context.FacultyAnnouncements.AddRangeAsync(announcements);
            await Context.SaveChangesAsync();
        }
    }
}
