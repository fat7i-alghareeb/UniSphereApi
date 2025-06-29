using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.DTOs.Announcements;

/// <summary>
/// DTO for creating faculty announcements with optional images
/// </summary>
public sealed record CreateFacultyAnnouncementWithImagesDto
{
    /// <summary>
    /// English title of the announcement
    /// </summary>
    public required string TitleEn { get; init; }
    
    /// <summary>
    /// Arabic title of the announcement
    /// </summary>
    public required string TitleAr { get; init; }
    
    /// <summary>
    /// English content of the announcement
    /// </summary>
    public required string ContentEn { get; init; }
    
    /// <summary>
    /// Arabic content of the announcement
    /// </summary>
    public required string ContentAr { get; init; }
    
    /// <summary>
    /// Optional list of image files to upload
    /// </summary>
    public List<IFormFile>? Images { get; init; }
} 