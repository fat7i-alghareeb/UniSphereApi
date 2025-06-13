using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UniSphere.Api.Database.Seeding;

public class DatabaseSeeder(
    ApplicationDbContext context,
    UniversitySeedData universitySeedData,
    FacultySeedData facultySeedData,
    FacultyAnnouncementSeedData facultyAnnouncementSeedData,
    MajorSeedData majorSeedData,
    MajorAnnouncementSeedData majorAnnouncementSeedData,
    EnrollmentStatusSeedData enrollmentStatusSeedData,
    ProfessorSeedData professorSeedData,
    SubjectSeedData subjectSeedData,
    StudentCredentialSeedData studentCredentialSeedData,
    SubjectProfessorLinkSeedData subjectProfessorLinkSeedData,
    SubjectStudentLinkSeedData subjectStudentLinkSeedData,
    LabSeedData labSeedData,
    InstructorSeedData instructorSeedData,
    InstructorLabLinkSeedData instructorLabLinkSeedData,
    ScheduleSeedData scheduleSeedData,
    LectureSeedData lectureSeedData,
    ILogger<DatabaseSeeder> logger)
{

    public async Task ClearAllDataAsync()
    {
        logger.LogInformation("Starting to delete all data from the database...");

        // Delete in reverse dependency order to avoid FK issues
        await context.Lectures.ExecuteDeleteAsync();
        await context.SubjectStudentLinks.ExecuteDeleteAsync();
        await context.SubjectProfessorLinks.ExecuteDeleteAsync();
        await context.StudentCredentials.ExecuteDeleteAsync();
        await context.Instructors.ExecuteDeleteAsync();
        await context.Labs.ExecuteDeleteAsync();
        await context.InstructorLabLink.ExecuteDeleteAsync();
        await context.MajorAnnouncements.ExecuteDeleteAsync();
        await context.Subjects.ExecuteDeleteAsync();
        await context.Professors.ExecuteDeleteAsync();
        await context.EnrollmentStatuses.ExecuteDeleteAsync();
        await context.Schedules.ExecuteDeleteAsync();
        await context.Majors.ExecuteDeleteAsync();
        await context.FacultyAnnouncements.ExecuteDeleteAsync();
        await context.Faculties.ExecuteDeleteAsync();
        await context.Universities.ExecuteDeleteAsync();
        await context.SaveChangesAsync();
        logger.LogInformation("All data deleted from the database.");
    }

    public async Task SeedAsync()
    {
        logger.LogInformation("Starting database seeding...");
        await context.Database.EnsureCreatedAsync();

        // Uncomment the next line if you want to clear all data before seeding
        // await ClearAllDataAsync();

        // Seed in the correct order based on dependencies
        await universitySeedData.SeedAsync();
        await facultySeedData.SeedAsync();
        await facultySeedData.SeedAsync();
        await facultyAnnouncementSeedData.SeedAsync();
        await majorSeedData.SeedAsync();
        await scheduleSeedData.SeedAsync();
        await enrollmentStatusSeedData.SeedAsync();
        await professorSeedData.SeedAsync();
        await subjectSeedData.SeedAsync();
        await majorAnnouncementSeedData.SeedAsync();
        await labSeedData.SeedAsync();
        await instructorSeedData.SeedAsync();
        await studentCredentialSeedData.SeedAsync();
        await subjectProfessorLinkSeedData.SeedAsync();
        await subjectStudentLinkSeedData.SeedAsync();
        await instructorLabLinkSeedData.SeedAsync();
        await lectureSeedData.SeedAsync();
        logger.LogInformation("Database seeding completed.");
    }
}
