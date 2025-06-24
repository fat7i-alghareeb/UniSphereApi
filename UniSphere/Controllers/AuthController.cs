using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Services;
using UniSphere.Api.Settings;

namespace UniSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthController(
    ApplicationDbContext applicationDbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options
) : BaseController
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    [HttpPost("Student/CheckOneTimeCode")]

    public async Task<ActionResult<SimpleStudentDto>> CheckOneTimeCode(StudentCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
      
        
        var studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.StudentNumber == checkOneTimeCodeDto.StudentNumber && sc.MajorId == checkOneTimeCodeDto.MajorId);
        if (studentCredential is null )
        {
            return NotFound();
        }

        if (!IsCodeValid(studentCredential, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(studentCredential.ToSimpleStudentDto());
    }

    [HttpPost("Student/Register")]
    public async Task<ActionResult<FullInfoStudentDto>> Register(RegisterStudentDto registerStudentDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        StudentCredential? studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            
            .FirstOrDefaultAsync(sc => sc.Id == registerStudentDto.StudentId);
        if (studentCredential is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerStudentDto.StudentId.ToString(),
            StudentId = registerStudentDto.StudentId,
        };
        if (registerStudentDto.Password != registerStudentDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createStudentResult = await userManager.CreateAsync(applicationUser, registerStudentDto.Password);
        if (!createStudentResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createStudentResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Student",
                title: "Error creating Student",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.Student);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createStudentResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Student",
                title: "Error creating Student",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        studentCredential.IdentityId = applicationUser.Id;
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Student], registerStudentDto.StudentId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("Student/Login")]
    public async Task<ActionResult<FullInfoStudentDto>> Login(LoginStudentDto loginStudentDto)
    {
        StudentCredential? studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc =>
                sc.StudentNumber == loginStudentDto.StudentNumber && sc.MajorId == loginStudentDto.MajorId);
        if (studentCredential is null)
        {
            return Problem(
                detail: "Student not found",
                title: "Student not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }


        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentCredential.Id);
        if (applicationUser is null )
        {
            return Problem(
                detail: "Student not Registered",
                title: "Student not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginStudentDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, studentCredential.Id)
        );
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == studentCredential.IdentityId);
        if (refreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = applicationUser.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
                Token = accessTokens.RefreshToken
            };
            await identityDbContext.RefreshTokens.AddAsync(newRefreshToken);
        }
        else
        {
            refreshToken.Token = accessTokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        }
        await identityDbContext.SaveChangesAsync();
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    // Admin Authentication Endpoints
    [HttpPost("Admin/CheckOneTimeCode")]
    public async Task<ActionResult<SimpleAdminDto>> CheckAdminOneTimeCode(AdminCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var admin = await applicationDbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Gmail == checkOneTimeCodeDto.Gmail && a.MajorId == checkOneTimeCodeDto.MajorId);
        if (admin is null)
        {
            return NotFound();
        }

        if (!IsAdminCodeValid(admin, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(admin.ToSimpleAdminDto());
    }

    [HttpPost("Admin/Register")]
    public async Task<ActionResult<FullInfoAdminDto>> RegisterAdmin(RegisterAdminDto registerAdminDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        Admin? admin = await applicationDbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == registerAdminDto.AdminId);
        if (admin is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerAdminDto.AdminId.ToString(),
            AdminId = registerAdminDto.AdminId,
        };
        if (registerAdminDto.Password != registerAdminDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createAdminResult = await userManager.CreateAsync(applicationUser, registerAdminDto.Password);
        if (!createAdminResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Admin",
                title: "Error creating Admin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.Admin);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Admin",
                title: "Error creating Admin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Admin], null, registerAdminDto.AdminId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("Admin/Login")]
    public async Task<ActionResult<FullInfoAdminDto>> LoginAdmin(LoginAdminDto loginAdminDto)
    {
        Admin? admin = await applicationDbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Gmail == loginAdminDto.Gmail && a.MajorId == loginAdminDto.MajorId);
        if (admin is null)
        {
            return Problem(
                detail: "Admin not found",
                title: "Admin not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.AdminId == admin.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "Admin not Registered",
                title: "Admin not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginAdminDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, admin.Id)
        );
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == applicationUser.Id);
        if (refreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = applicationUser.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
                Token = accessTokens.RefreshToken
            };
            await identityDbContext.RefreshTokens.AddAsync(newRefreshToken);
        }
        else
        {
            refreshToken.Token = accessTokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        }
        await identityDbContext.SaveChangesAsync();
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    // SuperAdmin Authentication Endpoints
    [HttpPost("SuperAdmin/CheckOneTimeCode")]
    public async Task<ActionResult<SimpleSuperAdminDto>> CheckSuperAdminOneTimeCode(SuperAdminCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var superAdmin = await applicationDbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Gmail == checkOneTimeCodeDto.Gmail && sa.FacultyId == checkOneTimeCodeDto.FacultyId);
        if (superAdmin is null)
        {
            return NotFound();
        }

        if (!IsSuperAdminCodeValid(superAdmin, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(superAdmin.ToSimpleSuperAdminDto());
    }

    [HttpPost("SuperAdmin/Register")]
    public async Task<ActionResult<FullInfoSuperAdminDto>> RegisterSuperAdmin(RegisterSuperAdminDto registerSuperAdminDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        SuperAdmin? superAdmin = await applicationDbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Id == registerSuperAdminDto.SuperAdminId);
        if (superAdmin is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerSuperAdminDto.SuperAdminId.ToString(),
            SuperAdminId = registerSuperAdminDto.SuperAdminId,
        };
        if (registerSuperAdminDto.Password != registerSuperAdminDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createSuperAdminResult = await userManager.CreateAsync(applicationUser, registerSuperAdminDto.Password);
        if (!createSuperAdminResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSuperAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SuperAdmin",
                title: "Error creating SuperAdmin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.SuperAdmin);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSuperAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SuperAdmin",
                title: "Error creating SuperAdmin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.SuperAdmin], null, null, registerSuperAdminDto.SuperAdminId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(superAdmin.ToFullInfoSuperAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("SuperAdmin/Login")]
    public async Task<ActionResult<FullInfoSuperAdminDto>> LoginSuperAdmin(LoginSuperAdminDto loginSuperAdminDto)
    {
        SuperAdmin? superAdmin = await applicationDbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Gmail == loginSuperAdminDto.Gmail && sa.FacultyId == loginSuperAdminDto.FacultyId);
        if (superAdmin is null)
        {
            return Problem(
                detail: "SuperAdmin not found",
                title: "SuperAdmin not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.SuperAdminId == superAdmin.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "SuperAdmin not Registered",
                title: "SuperAdmin not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginSuperAdminDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, null, superAdmin.Id)
        );
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == applicationUser.Id);
        if (refreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = applicationUser.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
                Token = accessTokens.RefreshToken
            };
            await identityDbContext.RefreshTokens.AddAsync(newRefreshToken);
        }
        else
        {
            refreshToken.Token = accessTokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        }
        await identityDbContext.SaveChangesAsync();
        return Ok(superAdmin.ToFullInfoSuperAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    // Professor Authentication Endpoints
    [HttpPost("Professor/CheckOneTimeCode")]
    public async Task<ActionResult<SimpleProfessorDto>> CheckProfessorOneTimeCode(ProfessorCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var professor = await applicationDbContext.Professors
            .FirstOrDefaultAsync(p => p.Gmail == checkOneTimeCodeDto.Gmail);
        if (professor is null)
        {
            return NotFound();
        }

        if (!IsProfessorCodeValid(professor, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(professor.ToSimpleProfessorDto());
    }

    [HttpPost("Professor/Register")]
    public async Task<ActionResult<FullInfoProfessorDto>> RegisterProfessor(RegisterProfessorDto registerProfessorDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        Professor? professor = await applicationDbContext.Professors
            .FirstOrDefaultAsync(p => p.Id == registerProfessorDto.ProfessorId);
        if (professor is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerProfessorDto.ProfessorId.ToString(),
            ProfessorId = registerProfessorDto.ProfessorId,
        };
        if (registerProfessorDto.Password != registerProfessorDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createProfessorResult = await userManager.CreateAsync(applicationUser, registerProfessorDto.Password);
        if (!createProfessorResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createProfessorResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Professor",
                title: "Error creating Professor",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.Professor);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createProfessorResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Professor",
                title: "Error creating Professor",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Professor], null, null, null, registerProfessorDto.ProfessorId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(professor.ToFullInfoProfessorDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("Professor/Login")]
    public async Task<ActionResult<FullInfoProfessorDto>> LoginProfessor(LoginProfessorDto loginProfessorDto)
    {
        Professor? professor = await applicationDbContext.Professors
            .FirstOrDefaultAsync(p => p.Gmail == loginProfessorDto.Gmail);
        if (professor is null)
        {
            return Problem(
                detail: "Professor not found",
                title: "Professor not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.ProfessorId == professor.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "Professor not Registered",
                title: "Professor not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginProfessorDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, null, null, professor.Id)
        );
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == applicationUser.Id);
        if (refreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = applicationUser.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
                Token = accessTokens.RefreshToken
            };
            await identityDbContext.RefreshTokens.AddAsync(newRefreshToken);
        }
        else
        {
            refreshToken.Token = accessTokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        }
        await identityDbContext.SaveChangesAsync();
        return Ok(professor.ToFullInfoProfessorDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    // SystemController Authentication Endpoints
    [HttpPost("SystemController/Login")]
    public async Task<ActionResult<FullInfoSystemControllerDto>> LoginSystemController(LoginSystemControllerDto loginSystemControllerDto)
    {
        SystemController? systemController = await applicationDbContext.SystemControllers
            .FirstOrDefaultAsync(sc => sc.Gmail == loginSystemControllerDto.Gmail);
        if (systemController is null)
        {
            return Problem(
                detail: "SystemController not found",
                title: "SystemController not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.SystemControllerId == systemController.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "SystemController not Registered",
                title: "SystemController not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginSystemControllerDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, null, null, null, systemController.Id)
        );
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == applicationUser.Id);
        if (refreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = applicationUser.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
                Token = accessTokens.RefreshToken
            };
            await identityDbContext.RefreshTokens.AddAsync(newRefreshToken);
        }
        else
        {
            refreshToken.Token = accessTokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        }
        await identityDbContext.SaveChangesAsync();
        return Ok(systemController.ToFullInfoSystemControllerDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("SystemController/Register")]
    public async Task<ActionResult<FullInfoSystemControllerDto>> RegisterSystemController(RegisterSystemControllerDto registerSystemControllerDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        SystemController? systemController = await applicationDbContext.SystemControllers
            .FirstOrDefaultAsync(sc => sc.Id == registerSystemControllerDto.SystemControllerId);
        if (systemController is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerSystemControllerDto.SystemControllerId.ToString(),
            SystemControllerId = registerSystemControllerDto.SystemControllerId,
        };
        if (registerSystemControllerDto.Password != registerSystemControllerDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createSystemControllerResult = await userManager.CreateAsync(applicationUser, registerSystemControllerDto.Password);
        if (!createSystemControllerResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSystemControllerResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SystemController",
                title: "Error creating SystemController",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.SystemController);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSystemControllerResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SystemController",
                title: "Error creating SystemController",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.SystemController], null, null, null, null, registerSystemControllerDto.SystemControllerId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(systemController.ToFullInfoSystemControllerDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<AccessTokensDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);
        if (refreshToken is null)
        {
            return Unauthorized();
        }
        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Unauthorized();
        }
        IList<string> roles = await userManager.GetRolesAsync(refreshToken.User);

        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, refreshToken.User.StudentId, refreshToken.User.AdminId, refreshToken.User.SuperAdminId, refreshToken.User.ProfessorId, refreshToken.User.SystemControllerId)
        );
        refreshToken.Token = accessTokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        await identityDbContext.SaveChangesAsync();
      
        return Ok(accessTokens);
    }

    private static bool IsCodeValid(StudentCredential studentCredential, int code)
    {
        if (studentCredential.OneTimeCodeCreatedDate is null ||
            studentCredential.OneTimeCodeExpirationInMinutes is null || studentCredential.OneTimeCode is null)
        {
            return false;
        }

        if (studentCredential.OneTimeCode != code)
        {
            return false;
        }

        if (studentCredential.OneTimeCodeCreatedDate!.Value.AddMinutes(studentCredential.OneTimeCodeExpirationInMinutes!
                .Value) < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    private static bool IsAdminCodeValid(Admin admin, int code)
    {
        if (admin.OneTimeCodeCreatedDate is null ||
            admin.OneTimeCodeExpirationInMinutes is null || admin.OneTimeCode is null)
        {
            return false;
        }

        if (admin.OneTimeCode != code)
        {
            return false;
        }

        if (admin.OneTimeCodeCreatedDate!.Value.AddMinutes(admin.OneTimeCodeExpirationInMinutes!
                .Value) < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    private static bool IsSuperAdminCodeValid(SuperAdmin superAdmin, int code)
    {
        if (superAdmin.OneTimeCodeCreatedDate is null ||
            superAdmin.OneTimeCodeExpirationInMinutes is null || superAdmin.OneTimeCode is null)
        {
            return false;
        }

        if (superAdmin.OneTimeCode != code)
        {
            return false;
        }

        if (superAdmin.OneTimeCodeCreatedDate!.Value.AddMinutes(superAdmin.OneTimeCodeExpirationInMinutes!
                .Value) < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }

    private static bool IsProfessorCodeValid(Professor professor, int code)
    {
        if (professor.OneTimeCodeCreatedDate is null ||
            professor.OneTimeCodeExpirationInMinutes is null || professor.OneTimeCode is null)
        {
            return false;
        }

        if (professor.OneTimeCode != code)
        {
            return false;
        }

        if (professor.OneTimeCodeCreatedDate!.Value.AddMinutes(professor.OneTimeCodeExpirationInMinutes!
                .Value) < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }
}
