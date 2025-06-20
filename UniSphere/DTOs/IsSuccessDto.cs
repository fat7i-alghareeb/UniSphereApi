namespace UniSphere.Api.DTOs;

public sealed record  IsSuccessDto
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
}
