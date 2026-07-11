namespace CareGuide.Models.Configuration;

public sealed class FileDownloadOptions
{
    public const string SectionName = "FileDownloads";

    public int PresignedUrlExpirationSeconds { get; init; } = 300;
}
