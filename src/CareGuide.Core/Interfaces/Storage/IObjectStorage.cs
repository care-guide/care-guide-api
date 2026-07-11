namespace CareGuide.Core.Interfaces.Storage;

public interface IObjectStorage
{
    Uri CreateUploadUrl(string objectKey, string contentType, TimeSpan expiresIn);
    Uri CreateDownloadUrl(string objectKey, TimeSpan expiresIn);
    Task<StorageObjectMetadata?> GetMetadataAsync(string objectKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);
}
