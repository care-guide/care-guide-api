namespace CareGuide.Models.DTOs.FileAsset;

public record InitiateEventFileUploadResponseDto(
    Guid FileId,
    Uri UploadUrl,
    string Method,
    IReadOnlyDictionary<string, string> RequiredHeaders,
    DateTimeOffset ExpiresAt);
