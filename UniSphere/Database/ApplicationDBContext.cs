using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options) 
{
    //public DbSet<MultilingualText> MultilingualTexts { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<StudentCredential> StudentCredentials { get; set; }
    public DbSet<SubjectProfessorLink> SubjectProfessorLinks { get; set; }
    public DbSet<SubjectStudentLink> SubjectStudentLinks { get; set; }
    public DbSet<Lab> Labs { get; set; }
    public DbSet<Lecture> Lectures { get; set; }
    public DbSet<Major> Majors { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<EnrollmentStatus> EnrollmentStatuses { get; set; }
    public DbSet<InstructorLabLink> InstructorLabLink { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<Professor> Professors { get; set; }
    public DbSet<FacultyAnnouncement> FacultyAnnouncements { get; set; }
    public DbSet<MajorAnnouncement> MajorAnnouncements { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemes.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
    
}
