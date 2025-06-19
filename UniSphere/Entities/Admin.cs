namespace UniSphere.Api.Entities;

public class Admin
{
    public required Guid Id { get; set; }
    public required Guid MajorId { get; set; }

    public required string Gmail { get; set; } = string.Empty;
    public int? OneTimeCode { get; set; }
    public DateTime? OneTimeCodeCreatedDate { get; set; }
    public int? OneTimeCodeExpirationInMinutes { get; init; }
    public required MultilingualText FirstName { get; set; } = new();
    public required MultilingualText LastName { get; set; } = new();
    public Major Major { get; set; } = new();
}
