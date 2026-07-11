namespace CareGuide.Models.Configuration;

public sealed class FileLifecycleOptions
{
    public const string SectionName = "FileLifecycle";

    public int PendingUploadExpirationMinutes { get; init; } = 60;
    public int UnreferencedFileRetentionDays { get; init; } = 30;
    public int CleanupIntervalMinutes { get; init; } = 15;
}
