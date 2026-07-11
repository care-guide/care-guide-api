using Amazon.S3;
using Amazon.S3.Model;
using CareGuide.Core.Interfaces.Storage;
using Microsoft.Extensions.Options;

namespace CareGuide.Infra.Storage;

public sealed class R2ObjectStorage(IAmazonS3 client, IOptions<R2StorageOptions> options) : IObjectStorage
{
    private readonly R2StorageOptions _options = options.Value;

    public Uri CreateUploadUrl(string objectKey, string contentType, TimeSpan expiresIn)
    {
        ValidateRequest(objectKey, expiresIn);
        if (string.IsNullOrWhiteSpace(contentType)) throw new ArgumentException("Content type is required.", nameof(contentType));

        return new Uri(client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.Add(expiresIn)
        }));
    }

    public Uri CreateDownloadUrl(string objectKey, TimeSpan expiresIn)
    {
        ValidateRequest(objectKey, expiresIn);
        return new Uri(client.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _options.Bucket,
            Key = objectKey,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.Add(expiresIn)
        }));
    }

    public async Task<StorageObjectMetadata?> GetMetadataAsync(string objectKey, CancellationToken cancellationToken = default)
    {
        ValidateObjectKey(objectKey);
        try
        {
            var response = await client.GetObjectMetadataAsync(_options.Bucket, objectKey, cancellationToken);
            return new StorageObjectMetadata(objectKey, response.ContentLength, response.Headers.ContentType,
                response.ETag, response.LastModified);
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
