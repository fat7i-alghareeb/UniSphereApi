using System;
using System.Collections.Generic;
namespace UniSphere.Api.Entities;

public class FacultyAnnouncement
{
    public Guid Id { get; set; }
    public MultilingualText Title { get; set; }
    public MultilingualText Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Faculty Faculty { get; set; }
    public Guid FacultyId { get; set; }
    public List<FacultyAnnouncementImage>? Images { get; set; } = new();
}
