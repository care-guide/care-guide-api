namespace CareGuide.Models.Configuration;

public sealed class FileUploadOptions
{
    public const string SectionName = "FileUploads";

    public long MaxSizeBytes { get; init; } = 10 * 1024 * 1024;
    public string[] AllowedContentTypes { get; init; } = [];
    public string[] AllowedExtensions { get; init; } = [];
    public string HashAlgorithm { get; init; } = "SHA256";
    public string HashFormat { get; init; } = "Hex";
    public int PresignedUrlExpirationSeconds { get; init; } = 300;
}
