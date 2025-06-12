using Microsoft.EntityFrameworkCore;

namespace UniSphere.Api.Database.Seeding;

public class DatabaseSeeder(
    ApplicationDbContext context,
    UniversitySeedData universitySeedData,
    FacultySeedData facultySeedData,
    MajorSeedData majorSeedData,
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
    LectureSeedData lectureSeedData)
{
    public async Task SeedAsync()
    {
        await context.Database.EnsureCreatedAsync();

        // Seed in the correct order based on dependencies
        await universitySeedData.SeedAsync();
        await facultySeedData.SeedAsync();
        await majorSeedData.SeedAsync();
        await scheduleSeedData.SeedAsync();
        await enrollmentStatusSeedData.SeedAsync();
        await professorSeedData.SeedAsync();
        await subjectSeedData.SeedAsync();
        await labSeedData.SeedAsync();
        await instructorSeedData.SeedAsync();
        await studentCredentialSeedData.SeedAsync();
        await subjectProfessorLinkSeedData.SeedAsync();
        await subjectStudentLinkSeedData.SeedAsync();
        await instructorLabLinkSeedData.SeedAsync();
        await lectureSeedData.SeedAsync();
    }
} 
