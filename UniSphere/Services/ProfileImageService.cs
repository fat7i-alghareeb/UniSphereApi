using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UniSphere.Api.Services;

public class ProfileImageService : IProfileImageService
{
    private readonly IStorageService _storageService;
    private readonly ILogger<ProfileImageService> _logger;
    private static readonly string[] AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

    public ProfileImageService(IStorageService storageService, ILogger<ProfileImageService> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public async Task<string> UploadProfileImageAsync(IFormFile image, string folder, long maxFileSize = 5 * 1024 * 1024)
    {
        if (image == null || image.Length == 0)
        {
            throw new InvalidOperationException("No image file provided");
        }

        var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(fileExtension))
        {
            throw new InvalidOperationException("Invalid image format. Allowed formats: jpg, jpeg, png, gif, bmp, webp");
        }

        if (image.Length > maxFileSize)
        {
            throw new InvalidOperationException($"Image file size must be less than {maxFileSize / (1024 * 1024)}MB");
        }

        // Save the image using the storage service
        var imageUrl = await _storageService.SaveFileAsync(image, folder);
        _logger.LogInformation("Profile image uploaded: {ImageUrl}", imageUrl);
        return imageUrl;
    }
} 