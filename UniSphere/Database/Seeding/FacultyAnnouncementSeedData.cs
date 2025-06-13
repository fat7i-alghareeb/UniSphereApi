using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class FacultyAnnouncementSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.FacultyAnnouncements.AnyAsync())
        {
            List<Faculty> faculties = await Context.Faculties
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

            foreach (Faculty faculty in faculties)
            {
                List<FacultyAnnouncement> facultyAnnouncements = faculty.Name.En switch
                {
                    "Faculty of Informatics Engineering" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "تنويه تقني", En = "Tech Notice" },
                            Content = new MultilingualText { Ar = "تم تحديث نظام تسجيل المقررات.", En = "The course registration system has been updated." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "إعلان ورشة برمجة", En = "Programming Workshop" },
                            Content = new MultilingualText { Ar = "ندعوكم لحضور ورشة عمل حول تطوير تطبيقات الويب.", En = "Join us for a web app development workshop." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "أمن المعلومات", En = "Cybersecurity Update" },
                            Content = new MultilingualText { Ar = "يرجى تغيير كلمات المرور الخاصة بحسابات الطلاب.", En = "Please update your student account passwords." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Mechanical and Electrical Engineering" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "صيانة مخابر", En = "Lab Maintenance" },
                            Content = new MultilingualText { Ar = "المخابر ستكون مغلقة للصيانة يوم الأحد.", En = "Labs will be closed for maintenance on Sunday." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "ندوة محركات", En = "Engines Seminar" },
                            Content = new MultilingualText { Ar = "محاضرة خاصة بمحركات الاحتراق الداخلي يوم الثلاثاء.", En = "Internal combustion engines lecture on Tuesday." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "ورشة طاقة متجددة", En = "Renewable Energy Workshop" },
                            Content = new MultilingualText { Ar = "دعوة لحضور ورشة الطاقة المتجددة.", En = "Invitation to attend renewable energy workshop." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Civil Engineering" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "إغلاق مؤقت", En = "Temporary Closure" },
                            Content = new MultilingualText { Ar = "مبنى الكلية سيُغلق مؤقتًا بسبب أعمال ترميم.", En = "The faculty building will be temporarily closed for renovation." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "رحلة ميدانية", En = "Field Trip" },
                            Content = new MultilingualText { Ar = "تنظم الكلية رحلة إلى موقع بناء في ضواحي دمشق.", En = "A field trip to a construction site near Damascus is planned." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "مؤتمر هندسي", En = "Engineering Conference" },
                            Content = new MultilingualText { Ar = "دعوة للمشاركة في مؤتمر الهندسة المدنية السنوي.", En = "Invitation to the annual civil engineering conference." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Architecture" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "مسابقة تصميم", En = "Design Competition" },
                            Content = new MultilingualText { Ar = "إطلاق مسابقة لأفضل تصميم داخلي.", En = "Launching a competition for best interior design." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "عرض مشاريع", En = "Project Exhibition" },
                            Content = new MultilingualText { Ar = "عرض مشاريع السنة النهائية الأسبوع المقبل.", En = "Final year projects exhibition next week." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "محاضرة ضيف", En = "Guest Lecture" },
                            Content = new MultilingualText { Ar = "محاضرة معمارية من ضيف دولي يوم الأربعاء.", En = "Architecture talk by an international guest on Wednesday." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Medicine" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "دورة إسعافات", En = "First Aid Course" },
                            Content = new MultilingualText { Ar = "تبدأ دورة الإسعافات الأولية يوم الإثنين.", En = "First aid course starts on Monday." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "مؤتمر علمي", En = "Medical Conference" },
                            Content = new MultilingualText { Ar = "مؤتمر حول التطورات الحديثة في الطب يوم الجمعة.", En = "Conference on modern medical advances on Friday." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "سحب دم تطوعي", En = "Blood Donation" },
                            Content = new MultilingualText { Ar = "ندعوكم للتبرع بالدم في بهو الكلية.", En = "Please donate blood in the faculty hall." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Dentistry" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "مخبر جديد", En = "New Lab Opening" },
                            Content = new MultilingualText { Ar = "افتتاح مخبر تشخيص جديد في الطابق الثاني.", En = "New diagnostic lab opened on the second floor." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "دورة تعقيم", En = "Sterilization Training" },
                            Content = new MultilingualText { Ar = "ورشة حول إجراءات التعقيم الحديثة.", En = "Workshop on modern sterilization procedures." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "إعلانات هامة", En = "Important Notices" },
                            Content = new MultilingualText { Ar = "تحديث حول استخدام المعدات السنية.", En = "Update regarding dental equipment usage." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Pharmacy" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "محاضرة دوائية", En = "Pharmacology Lecture" },
                            Content = new MultilingualText { Ar = "محاضرة حول الأدوية الجديدة في السوق.", En = "Lecture on new market drugs." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "تدريب صيدلي", En = "Pharmacy Training" },
                            Content = new MultilingualText { Ar = "تدريب عملي في صيدليات الجامعة.", En = "Practical training in campus pharmacies." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "إعلان مختبرات", En = "Lab Notice" },
                            Content = new MultilingualText { Ar = "يرجى إحضار المعاطف البيضاء لمختبر الغد.", En = "Please bring lab coats for tomorrow's session." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Science" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "أسبوع العلوم", En = "Science Week" },
                            Content = new MultilingualText { Ar = "فعاليات أسبوع العلوم تبدأ الأحد.", En = "Science week activities start Sunday." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "ندوة كيميائية", En = "Chemistry Seminar" },
                            Content = new MultilingualText { Ar = "ندوة خاصة حول المواد الكيميائية النانوية.", En = "Special seminar on nano-chemical materials." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "مشروع أبحاث", En = "Research Projects" },
                            Content = new MultilingualText { Ar = "يرجى تقديم مقترحات مشاريع البحث.", En = "Please submit your research project proposals." },
                            FacultyId = faculty.Id
                        }
                    },

                    "Faculty of Economics" => new()
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "تحليل سوق", En = "Market Analysis" },
                            Content = new MultilingualText { Ar = "ورشة حول تحليل السوق المالية المحلية.", En = "Workshop on analyzing the local financial market." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "مراجعة اختبارات", En = "Exam Review" },
                            Content = new MultilingualText { Ar = "جلسة مراجعة لاختبار الاقتصاد الكلي.", En = "Macroeconomics exam review session." },
                            FacultyId = faculty.Id
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Title = new MultilingualText { Ar = "فرص تدريب", En = "Internship Offers" },
                            Content = new MultilingualText { Ar = "فرص تدريبية في بنوك محلية متاحة الآن.", En = "Internship opportunities now available in local banks." },
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
