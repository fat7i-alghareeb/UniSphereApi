using System;
using System.Collections.Generic;

namespace UniSphere.Api.Entities;

public class MajorAnnouncement
{
    public Guid Id { get; set; }
    public MultilingualText Title { get; set; }
    public MultilingualText Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int Year { get; set; }
    public Subject Subject { get; set; }
    public Major Major { get; set; }
    public Guid SubjectId { get; set; }
    public Guid MajorId { get; set; }
}
