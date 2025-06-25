namespace UniSphere.Api.Entities;

public class SystemController {
        public required Guid Id { get; set; }
        public required  string Gmail { get; set; } = string.Empty;
        public required  string UserName { get; set; } = string.Empty;
        public string? IdentityId { get; set; }
}
