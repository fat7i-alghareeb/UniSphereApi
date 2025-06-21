namespace UniSphere.Api.DTOs.Schedule;

public sealed record AvailableLabsCollectionDto
{
    public List<AvailableLabDto> Labs { get; set; } = new List<AvailableLabDto>(); 
}
