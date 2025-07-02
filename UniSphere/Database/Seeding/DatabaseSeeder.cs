using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class DatabaseSeeder(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext applicationIdentityDbContext,
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
    StudentStatisticsSeedData studentStatisticsSeedData,
    AdminSeedData adminSeedData,
    SuperAdminSeedData superAdminSeedData,
    ILogger<DatabaseSeeder> logger,
    RoleManager<IdentityRole> roleManager
)
{
    public async Task ClearApplicationDataAsync()
    {
        logger.LogInformation("Starting to delete all data from the database...");

        // Delete in reverse dependency order to avoid FK issues
        await applicationDbContext.SuperAdmins.ExecuteDeleteAsync();
        await applicationDbContext.Admins.ExecuteDeleteAsync();
        await applicationDbContext.Lectures.ExecuteDeleteAsync();
        await applicationDbContext.SubjectStudentLinks.ExecuteDeleteAsync();
        await applicationDbContext.SubjectProfessorLinks.ExecuteDeleteAsync();
        await applicationDbContext.StudentStatistics.ExecuteDeleteAsync();
        await applicationDbContext.StudentCredentials.ExecuteDeleteAsync();
        await applicationDbContext.Instructors.ExecuteDeleteAsync();
        await applicationDbContext.Labs.ExecuteDeleteAsync();
        await applicationDbContext.InstructorLabLink.ExecuteDeleteAsync();
        await applicationDbContext.MajorAnnouncements.ExecuteDeleteAsync();
        await applicationDbContext.Subjects.ExecuteDeleteAsync();
        await applicationDbContext.Professors.ExecuteDeleteAsync();
        await applicationDbContext.EnrollmentStatuses.ExecuteDeleteAsync();
        await applicationDbContext.Schedules.ExecuteDeleteAsync();
        await applicationDbContext.Majors.ExecuteDeleteAsync();
        await applicationDbContext.FacultyAnnouncements.ExecuteDeleteAsync();
        await applicationDbContext.Faculties.ExecuteDeleteAsync();
        await applicationDbContext.Universities.ExecuteDeleteAsync();
        await applicationDbContext.SaveChangesAsync();
        logger.LogInformation("All data deleted from the database.");
    }

    public async Task ClearIdentityDataAsync()
    {
        logger.LogInformation("Starting to delete all identity data from the database...");
        await applicationIdentityDbContext.UserTokens.ExecuteDeleteAsync();
        await applicationIdentityDbContext.UserClaims.ExecuteDeleteAsync();
        await applicationIdentityDbContext.RefreshTokens.ExecuteDeleteAsync();
        await applicationIdentityDbContext.UserLogins.ExecuteDeleteAsync();
        await applicationIdentityDbContext.Users.ExecuteDeleteAsync();
        await applicationIdentityDbContext.RoleClaims.ExecuteDeleteAsync();
        await applicationIdentityDbContext.Roles.ExecuteDeleteAsync();
        await applicationIdentityDbContext.UserRoles.ExecuteDeleteAsync();
        await applicationIdentityDbContext.SaveChangesAsync();
        logger.LogInformation("All identity data deleted from the database.");
    }

    public async Task SeedAsync()
    {
        logger.LogInformation("Starting database seeding...");
        await applicationDbContext.Database.EnsureCreatedAsync();

        // Clear all data before seeding to avoid conflicts
      //  await ClearApplicationDataAsync();

        // Seed in the correct order based on dependencies
        await universitySeedData.SeedAsync();
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
        await studentStatisticsSeedData.SeedAsync();
        await subjectProfessorLinkSeedData.SeedAsync();
        await subjectStudentLinkSeedData.SeedAsync();
        await instructorLabLinkSeedData.SeedAsync();
        await lectureSeedData.SeedAsync();
        await superAdminSeedData.SeedAsync();
        await adminSeedData.SeedAsync();
        logger.LogInformation("Database seeding completed.");
    }

    public async Task SeedRolesAsync()
    {
        try
        {
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Professor))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Professor));
            }

            if (!await roleManager.RoleExistsAsync(Roles.Student))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Student));
            }

            if (!await roleManager.RoleExistsAsync(Roles.SuperAdmin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seeding roles failed.");
        }
    }
}
