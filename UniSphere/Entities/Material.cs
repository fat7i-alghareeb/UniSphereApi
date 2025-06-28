namespace UniSphere.Api.Entities;

public class Material
{
    public required Guid Id { get; set; }
    public required Guid SubjectId { get; set; }
    public required string Url { get; set; }
    public required string Type { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Subject Subject { get; set; } = null!;
} 