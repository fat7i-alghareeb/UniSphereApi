namespace UniSphere.Api.DTOs.Auth;

public sealed record TokenRequest(IEnumerable<string> Roles, Guid? StudentId = null, Guid? AdminId = null, Guid? SuperAdminId = null, Guid? ProfessorId = null, Guid? SystemControllerId = null);

