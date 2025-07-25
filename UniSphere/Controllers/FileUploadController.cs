using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniSphere.Api.Services;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

/// <summary>
/// Controller for handling file uploads
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IStorageService storageService, ILogger<FileUploadController> logger)
    : BaseController
{
    private readonly IStorageService _storageService =
        storageService ?? throw new ArgumentNullException(nameof(storageService));

    private readonly ILogger<FileUploadController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Upload a file with automatic folder detection based on file extension
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <returns>The URL path to the uploaded file</returns>
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
     

            // The service will automatically determine the folder based on file extension
            var fileUrl = await _storageService.SaveFileAsync(file);

            _logger.LogInformation("File uploaded successfully: {FileUrl}", fileUrl);

            return Ok(new
            {
                message = BilingualErrorMessages.GetSuccessMessage(Lang),
                fileUrl,
                fileName = file.FileName,
                fileSize = file.Length,
                contentType = file.ContentType
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", file?.FileName);
            return BadRequest(new { message = BilingualErrorMessages.GetFileUploadErrorMessage(Lang) });
        }
    }

    /// <summary>
    /// Upload a file to a specific custom folder
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="folder">The custom folder where to save the file</param>
    /// <returns>The URL path to the uploaded file</returns>
    [HttpPost("upload-custom")]
    public async Task<IActionResult> UploadFileToCustomFolder(IFormFile file, [FromQuery] string folder)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(folder))
            {
                return BadRequest(new { message = BilingualErrorMessages.GetBadRequestMessage(Lang) });
            }

            var fileUrl = await _storageService.SaveFileAsync(file, folder);

            _logger.LogInformation("File uploaded successfully to custom folder: {FileUrl}", fileUrl);

            return Ok(new
            {
                message = BilingualErrorMessages.GetSuccessMessage(Lang),
                fileUrl,
                fileName = file.FileName,
                fileSize = file.Length,
                contentType = file.ContentType,
                customFolder = folder
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName} to custom folder {Folder}", file?.FileName, folder);
            return BadRequest(new { message = BilingualErrorMessages.GetFileUploadErrorMessage(Lang) });
        }
    }

    /// <summary>
    /// Upload multiple files with automatic folder detection
    /// </summary>
    /// <param name="files">The files to upload</param>
    /// <returns>The URL paths to the uploaded files</returns>
    [HttpPost("upload-multiple")]
    public async Task<IActionResult> UploadMultipleFiles(IFormFileCollection files)
    {
        try
        {
            var uploadedFiles = new List<object>();

            foreach (var file in files)
            {
                try
                {
                    var fileUrl = await _storageService.SaveFileAsync(file);
                    uploadedFiles.Add(new
                    {
                        originalName = file.FileName,
                        fileUrl,
                        fileSize = file.Length,
                        contentType = file.ContentType
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
                    uploadedFiles.Add(new
                    {
                        originalName = file.FileName,
                        error = ex.Message
                    });
                }
            }

            return Ok(new
            {
                message = BilingualErrorMessages.GetSuccessMessage(Lang), 
                uploadedFiles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing multiple file uploads");
            return BadRequest(new { message = BilingualErrorMessages.GetFileUploadErrorMessage(Lang) });
        }
    }

    /// <summary>
    /// Get supported file types and their extensions
    /// </summary>
    /// <returns>Dictionary of supported file types and their extensions</returns>
    [HttpGet("supported-types")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetSupportedFileTypes()
    {
        try
        {
            var supportedTypes = LocalStorageService.GetSupportedFileTypes();
            return Ok(new
            {
                message = BilingualErrorMessages.GetSuccessMessage(Lang),
                supportedTypes
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving supported file types");
            return BadRequest(new { message = BilingualErrorMessages.GetInternalServerErrorMessage(Lang) });
        }
    }
}
