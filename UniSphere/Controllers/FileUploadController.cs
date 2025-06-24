// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using UniSphere.Api.Services;

// namespace UniSphere.Api.Controllers;

// /// <summary>
// /// Controller for handling file uploads
// /// </summary>
// [ApiController]
// [Route("api/[controller]")]
// public class FileUploadController : ControllerBase
// {
//     private readonly IStorageService _storageService;
//     private readonly ILogger<FileUploadController> _logger;

//     public FileUploadController(IStorageService storageService, ILogger<FileUploadController> logger)
//     {
//         _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
//         _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//     }

//     /// <summary>
//     /// Upload a file with automatic folder detection based on file extension
//     /// </summary>
//     /// <param name="file">The file to upload</param>
//     /// <returns>The URL path to the uploaded file</returns>
//     [HttpPost("upload")]
//     [Authorize] // Add authorization as needed
//     public async Task<IActionResult> UploadFile(IFormFile file)
//     {
//         try
//         {
//             if (file == null)
//                 return BadRequest("No file was uploaded");

//             // The service will automatically determine the folder based on file extension
//             var fileUrl = await _storageService.SaveFileAsync(file);
            
//             _logger.LogInformation("File uploaded successfully: {FileUrl}", fileUrl);
            
//             return Ok(new { 
//                 message = "File uploaded successfully", 
//                 fileUrl = fileUrl,
//                 fileName = file.FileName,
//                 fileSize = file.Length,
//                 contentType = file.ContentType
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error uploading file {FileName}", file?.FileName);
//             return BadRequest(new { message = ex.Message });
//         }
//     }

//     /// <summary>
//     /// Upload a file to a specific custom folder
//     /// </summary>
//     /// <param name="file">The file to upload</param>
//     /// <param name="folder">The custom folder where to save the file</param>
//     /// <returns>The URL path to the uploaded file</returns>
//     [HttpPost("upload-custom")]
//     [Authorize] // Add authorization as needed
//     public async Task<IActionResult> UploadFileToCustomFolder(IFormFile file, [FromQuery] string folder)
//     {
//         try
//         {
//             if (file == null)
//                 return BadRequest("No file was uploaded");

//             if (string.IsNullOrWhiteSpace(folder))
//                 return BadRequest("Folder name is required");

//             var fileUrl = await _storageService.SaveFileAsync(file, folder);
            
//             _logger.LogInformation("File uploaded successfully to custom folder: {FileUrl}", fileUrl);
            
//             return Ok(new { 
//                 message = "File uploaded successfully to custom folder", 
//                 fileUrl,
//                 fileName = file.FileName,
//                 fileSize = file.Length,
//                 contentType = file.ContentType,
//                 customFolder = folder
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error uploading file {FileName} to custom folder {Folder}", file?.FileName, folder);
//             return BadRequest(new { message = ex.Message });
//         }
//     }

//     /// <summary>
//     /// Upload multiple files with automatic folder detection
//     /// </summary>
//     /// <param name="files">The files to upload</param>
//     /// <returns>The URL paths to the uploaded files</returns>
//     [HttpPost("upload-multiple")]
//     [Authorize] // Add authorization as needed
//     public async Task<IActionResult> UploadMultipleFiles(IFormFileCollection files)
//     {
//         try
//         {
//             if (files == null || files.Count == 0)
//                 return BadRequest("No files were uploaded");

//             var uploadedFiles = new List<object>();

//             foreach (var file in files)
//             {
//                 try
//                 {
//                     var fileUrl = await _storageService.SaveFileAsync(file);
//                     uploadedFiles.Add(new
//                     {
//                         originalName = file.FileName,
//                         fileUrl = fileUrl,
//                         fileSize = file.Length,
//                         contentType = file.ContentType
//                     });
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
//                     uploadedFiles.Add(new
//                     {
//                         originalName = file.FileName,
//                         error = ex.Message
//                     });
//                 }
//             }

//             return Ok(new { 
//                 message = "Files processed", 
//                 uploadedFiles = uploadedFiles 
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error processing multiple file uploads");
//             return BadRequest(new { message = ex.Message });
//         }
//     }

//     /// <summary>
//     /// Get supported file types and their extensions
//     /// </summary>
//     /// <returns>Dictionary of supported file types and their extensions</returns>
//     [HttpGet("supported-types")]
//     public IActionResult GetSupportedFileTypes()
//     {
//         try
//         {
//             var supportedTypes = LocalStorageService.GetSupportedFileTypes();
//             return Ok(new { 
//                 message = "Supported file types retrieved successfully",
//                  supportedTypes
//             });
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "Error retrieving supported file types");
//             return BadRequest(new { message = ex.Message });
//         }
//     }
// } 