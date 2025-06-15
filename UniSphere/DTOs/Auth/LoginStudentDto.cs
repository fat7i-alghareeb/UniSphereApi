namespace UniSphere.Api.DTOs.Auth;

public sealed record class LoginStudentDto
{
    
        public required string StudentNumber{ get; init; }
        public required Guid MajorId{ get; init; }
        public required string Password{ get; init; }
}
    
