using UniSphere.Api.DTOs.Announcements;
using UniSphere.Api.DTOs.Statistics;

namespace UniSphere.Api.DTOs.Info;

public sealed record HomeDto
{
    public required List<Top10FacultyAnnouncementsDto> Announcements { get; init; }
    public required DateTime DaysToTheFinal { get; init; }
    public required StatisticsDto Statistics { get; init; }
    
}


