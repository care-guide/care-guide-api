namespace CareGuide.Models.DTOs.EventFile;

public record EventFileDownloadDto(
    Guid FileId,
    Uri DownloadUrl,
    DateTimeOffset ExpiresAt);
