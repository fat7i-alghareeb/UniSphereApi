﻿namespace UniSphere.Api.Entities;

public class SuperAdmin
{
    public required Guid Id { get; set; }
    public required   Guid FacultyId { get; set; }
    public required  string Gmail { get; set; } = string.Empty;
    public int? OneTimeCode { get; set; }
    public DateTime? OneTimeCodeCreatedDate { get; set; }
    public int? OneTimeCodeExpirationInMinutes { get; set; }
    public required  MultilingualText FirstName { get; set; }=new();
    public required  MultilingualText LastName { get; set; }=new();
    public string? Image { get; set; }
    public Faculty Faculty { get; set; } = null!;
    public string? IdentityId { get; set; }
}
