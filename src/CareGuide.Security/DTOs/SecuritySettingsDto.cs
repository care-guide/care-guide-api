namespace CareGuide.Security.DTOs
{
    public class SecuritySettingsDto
    {
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public string? Audience { get; set; }
    }
}
