using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace UniSphere.Api.Services;

public interface IProfileImageService
{
    /// <summary>
    /// Validates and uploads a profile image for a user.
    /// </summary>
    /// <param name="image">The image file to upload.</param>
    /// <param name="folder">The folder to save the image in.</param>
    /// <param name="maxFileSize">Maximum allowed file size in bytes.</param>
    /// <returns>The URL of the uploaded image.</returns>
    Task<string> UploadProfileImageAsync(IFormFile image, string folder, long maxFileSize = 5 * 1024 * 1024);
} 