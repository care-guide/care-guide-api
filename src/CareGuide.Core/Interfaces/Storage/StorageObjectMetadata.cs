namespace CareGuide.Core.Interfaces.Storage;

public sealed record StorageObjectMetadata(
    string ObjectKey,
    long ContentLength,
    string? ContentType,
    string? ETag,
    string? ExpectedSha256,
    DateTime? LastModified);
