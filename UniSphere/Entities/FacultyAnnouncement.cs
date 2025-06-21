using System;
using System.Collections.Generic;
using UniSphere.Api.DTOs.Announcements;

namespace UniSphere.Api.Entities;

public class FacultyAnnouncement
{
    public Guid Id { get; set; }
    public MultilingualText Title { get; set; }
    public MultilingualText Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Faculty Faculty { get; set; }
    public Guid FacultyId { get; set; }
    //public List<Image>? Images { get; set; } = new();
    
}
