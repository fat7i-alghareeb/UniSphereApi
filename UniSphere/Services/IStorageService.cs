using Microsoft.AspNetCore.Http;

namespace UniSphere.Api.Services;

public interface IStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folder = "");
} 