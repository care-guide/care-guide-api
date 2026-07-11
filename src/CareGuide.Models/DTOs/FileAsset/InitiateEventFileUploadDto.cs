namespace CareGuide.Models.DTOs.FileAsset;

public record InitiateEventFileUploadDto(
    string Name,
    long Size,
    string ContentType,
    string ExpectedHash);
