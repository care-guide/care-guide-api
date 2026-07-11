namespace CareGuide.Infra.Storage;

public sealed class R2StorageOptions
{
    public const string SectionName = "Storage:R2";

    public string Endpoint { get; init; } = string.Empty;
    public string AccountId { get; init; } = string.Empty;
    public string Bucket { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public int MinPresignedUrlExpirationSeconds { get; init; } = 60;
    public int MaxPresignedUrlExpirationSeconds { get; init; } = 3600;
}
