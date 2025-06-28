
namespace UniSphere.Api.Services;

/// <summary>
/// Local file storage service implementation
/// </summary>
public class LocalStorageService(IWebHostEnvironment environment, ILogger<LocalStorageService> logger)
    : IStorageService
{
    private readonly IWebHostEnvironment _environment = environment ?? throw new ArgumentNullException(nameof(environment));
    private readonly ILogger<LocalStorageService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private const string UploadsFolder = "uploads";
    private const long MaxFileSize = 30 * 1024 * 1024; // 30MB
    
    // File type mappings with their allowed extensions and target folders
    private static readonly Dictionary<string, (string[] Extensions, string Folder)> FileTypeMappings = new()
    {
        ["images"] = (new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg" }, "images"),
        ["pdfs"] = (new[] { ".pdf" }, "pdfs"),
        ["excel"] = (new[] { ".xlsx", ".xls", ".csv" }, "excel"),
        ["documents"] = (new[] { ".doc", ".docx", ".txt", ".rtf" }, "documents")
    };

    // Flattened list of all allowed extensions for validation
    private static readonly string[] AllowedExtensions = FileTypeMappings
        .SelectMany(mapping => mapping.Value.Extensions)
        .ToArray();

    // Static readonly field for invalid path characters
    private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars().Concat(new[] { '/', '\\' }).ToArray();

    /// <summary>
    /// Saves an uploaded file to a folder determined by its extension
    /// </summary>
    /// <param name="file">The file to be saved</param>
    /// <param name="folder">Optional custom folder (if not provided, will be determined by file extension)</param>
    /// <returns>The relative URL path to the saved file</returns>
    public async Task<string> SaveFileAsync(IFormFile file, string folder = "")
    {
        try
        {
            // Validate input parameters
            if (file == null){

                throw new ArgumentNullException(nameof(file), "File cannot be null");
            }

            // Validate file
            ValidateFileAsync(file);

            // Determine target folder based on file extension if not provided
            var targetFolder = string.IsNullOrWhiteSpace(folder) 
                ? GetFolderByExtension(file.FileName) 
                : folder;

            // Sanitize folder name
            var sanitizedFolder = SanitizeFolderName(targetFolder);

            // Create the full directory path
            var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder, sanitizedFolder);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
                _logger.LogInformation("Created directory: {DirectoryPath}", uploadsPath);
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("File saved successfully: {FilePath} in folder {Folder}", filePath, sanitizedFolder);

            // Return the relative URL path
            return $"/{UploadsFolder}/{sanitizedFolder}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName} to folder {Folder}", file?.FileName, folder);
            throw;
        }
    }

    /// <summary>
    /// Determines the appropriate folder based on file extension
    /// </summary>
    /// <param name="fileName">The name of the file</param>
    /// <returns>The folder name for the file type</returns>
    private static string GetFolderByExtension(string fileName)
    {
        var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
        
        foreach (var mapping in FileTypeMappings)
        {
            if (mapping.Value.Extensions.Contains(fileExtension))
            {
                return mapping.Value.Folder;
            }
        }
        
        // Default to documents folder for unknown extensions
        return "documents";
    }

    /// <summary>
    /// Gets all supported file types and their extensions
    /// </summary>
    /// <returns>Dictionary of file types and their extensions</returns>
    public static Dictionary<string, string[]> GetSupportedFileTypes()
    {
        return FileTypeMappings.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Extensions
        );
    }

    /// <summary>
    /// Validates the uploaded file
    /// </summary>
    /// <param name="file">The file to validate</param>
    private void ValidateFileAsync(IFormFile file)
    {
        // Check if file has content
        if (file.Length == 0)
        {
            throw new InvalidOperationException("File is empty");
        }

        // Check file size
        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");
        }

        // Check file extension
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(fileExtension) || !AllowedExtensions.Contains(fileExtension))
        {
            var supportedTypes = string.Join(", ", FileTypeMappings.Select(m => $"{m.Key} ({string.Join(", ", m.Value.Extensions)})"));
            throw new InvalidOperationException($"File type not allowed. Supported types: {supportedTypes}");
        }

        // Check MIME type (basic validation)
        if (string.IsNullOrEmpty(file.ContentType))
        {
            throw new InvalidOperationException("File content type is not specified");
        }
    }

    /// <summary>
    /// Sanitizes the folder name to prevent directory traversal attacks
    /// </summary>
    /// <param name="folderName">The folder name to sanitize</param>
    /// <returns>The sanitized folder name</returns>
    private static string SanitizeFolderName(string folderName)
    {
        // Remove any path separators and invalid characters
        var sanitized = string.Join("", folderName.Split(InvalidPathChars, StringSplitOptions.RemoveEmptyEntries));
        
        // Remove any remaining dangerous characters
        sanitized = sanitized.Replace("..", "").Replace("~", "");
        
        // Ensure the folder name is not empty after sanitization
        if (string.IsNullOrWhiteSpace(sanitized))
        {
            sanitized = "documents";
        }
        return sanitized.ToLowerInvariant();
    }

    /// <summary>
    /// Infers the material type from a file URL or path
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:URI return values should not be strings", Justification = "Returns a type label, not a URI.")]
    public static string GetMaterialTypeFromUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) { return "unknown"; }
        var ext = Path.GetExtension(url).ToLowerInvariant();
        return ext switch
        {
            ".pdf" => "pdf",
            ".doc" or ".docx" => "document",
            ".xls" or ".xlsx" or ".csv" => "excel",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" or ".svg" => "image",
            _ => ext.TrimStart('.')
        };
    }
} 
