using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.Entities;

public class University
{
    public Guid Id { get; set; }
    public MultilingualText Name { get; set; } = new();
    public MultilingualText Type { get; set; } = new();
    public List<Faculty> Faculties { get; set; } = new();
} 