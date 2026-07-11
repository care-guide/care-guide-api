namespace CareGuide.Core.Interfaces.Storage;

public interface IObjectStorage
{
    Uri CreateUploadUrl(string objectKey, string contentType, string expectedSha256, TimeSpan expiresIn);
    Uri CreateDownloadUrl(string objectKey, string downloadFileName, TimeSpan expiresIn);
    Task<StorageObjectMetadata?> GetMetadataAsync(string objectKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
}
