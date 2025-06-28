using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.DTOs.Subjects;

/// <summary>
/// DTO for uploading materials to subjects. Supports both file uploads and link uploads.
/// </summary>
public sealed record UploadMaterialDto
{
    /// <summary>
    /// File to upload. Use this for file uploads (PDF, images, documents, etc.)
    /// </summary>
    public IFormFile? File { get; init; }
    
    /// <summary>
    /// URL link to external resource. Use this for links (YouTube, blogs, Google Docs, etc.)
    /// </summary>
    public string? Link { get; init; }
    
    /// <summary>
    /// Optional custom type override. If not provided, type will be auto-detected from file extension or URL.
    /// Examples: "youtube", "blog", "pdf", "document", "video", etc.
    /// </summary>
    public string? CustomType { get; init; }
    
    /// <summary>
    /// Validates that either a file or a link is provided
    /// </summary>
    /// <returns>True if either file or link is provided, false otherwise</returns>
    public bool IsValid()
    {
        return File != null && File.Length > 0 || !string.IsNullOrWhiteSpace(Link);
    }
    
    /// <summary>
    /// Indicates if this is a file upload
    /// </summary>
    public bool IsFileUpload => File != null && File.Length > 0;
    
    /// <summary>
    /// Indicates if this is a link upload
    /// </summary>
    public bool IsLinkUpload => !string.IsNullOrWhiteSpace(Link);
} 