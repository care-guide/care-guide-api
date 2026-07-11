using Amazon.S3;
using Amazon.S3.Model;
using CareGuide.Core.Interfaces.Storage;
using Microsoft.Extensions.Options;

namespace CareGuide.Infra.Storage;

public sealed class R2ObjectStorage(IAmazonS3 client, IOptions<R2StorageOptions> options) : IObjectStorage
{
    private readonly R2StorageOptions _options = options.Value;

    public Uri CreateUploadUrl(string objectKey, string contentType, string expectedSha256, TimeSpan expiresIn)
    {
        ValidateRequest(objectKey, expiresIn);
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("Content type is required.", nameof(contentType));
        if (string.IsNullOrWhiteSpace(expectedSha256)) throw new ArgumentException("Expected SHA-256 is required.", nameof(expectedSha256));

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.Add(expiresIn)
        };
        request.Metadata.Add("sha256", expectedSha256);
        return new Uri(client.GetPreSignedURL(request));
    }

    public Uri CreateDownloadUrl(string objectKey, string downloadFileName, TimeSpan expiresIn)
    {
        ValidateRequest(objectKey, expiresIn);
        if (string.IsNullOrWhiteSpace(downloadFileName)) throw new ArgumentException("Download file name is required.", nameof(downloadFileName));
        var safeFileName = Path.GetFileName(downloadFileName);
        if (safeFileName.Any(char.IsControl)) throw new ArgumentException("Download file name contains invalid characters.", nameof(downloadFileName));

        return new Uri(client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiresIn),
            ResponseHeaderOverrides = new ResponseHeaderOverrides
            {
                ContentDisposition = $"attachment; filename*=UTF-8''{Uri.EscapeDataString(safeFileName)}"
            }
        }));
    }

    public async Task<StorageObjectMetadata?> GetMetadataAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        ValidateObjectKey(objectKey);
        try
        {
            var response = await client.GetObjectMetadataAsync(_options.Bucket, objectKey, cancellationToken);
            return new StorageObjectMetadata(objectKey, response.ContentLength, response.Headers.ContentType,
                response.ETag, response.Metadata["x-amz-meta-sha256"], response.LastModified);
        }
        catch (AmazonS3Exception exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        ValidateObjectKey(objectKey);
        await client.DeleteObjectAsync(_options.Bucket, objectKey, cancellationToken);
    }

    private void ValidateRequest(string objectKey, TimeSpan expiresIn)
    {
        ValidateObjectKey(objectKey);
        var seconds = expiresIn.TotalSeconds;
        if (seconds < _options.MinPresignedUrlExpirationSeconds || seconds > _options.MaxPresignedUrlExpirationSeconds)
            throw new ArgumentOutOfRangeException(nameof(expiresIn), "Expiration is outside the configured allowed range.");
    }

    private static void ValidateObjectKey(string objectKey)
    {
        if (string.IsNullOrWhiteSpace(objectKey)) throw new ArgumentException("Object key is required.", nameof(objectKey));
    }
}
