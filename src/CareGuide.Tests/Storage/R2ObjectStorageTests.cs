using Amazon.S3;
using Amazon.S3.Model;
using CareGuide.Infra.Storage;
using Microsoft.Extensions.Options;

namespace CareGuide.Tests.Storage;

public class R2ObjectStorageTests
{
    private readonly IAmazonS3 _client = Substitute.For<IAmazonS3>();

    [Fact]
    public void CreateUploadUrl_ShouldUseKeyContentTypeAndConfiguredBucket()
    {
        GetPreSignedUrlRequest? captured = null;
        _client.GetPreSignedURL(Arg.Do<GetPreSignedUrlRequest>(request => captured = request))
            .Returns("https://example.test/upload");
        var storage = CreateStorage();

        var result = storage.CreateUploadUrl("person/file.pdf", "application/pdf", new string('a', 64), TimeSpan.FromMinutes(5));

        result.Should().Be(new Uri("https://example.test/upload"));
        captured.Should().NotBeNull();
        captured!.BucketName.Should().Be("private-bucket");
        captured.Key.Should().Be("person/file.pdf");
        captured.ContentType.Should().Be("application/pdf");
        captured.Metadata["x-amz-meta-sha256"].Should().Be(new string('a', 64));
        captured.Verb.Should().Be(HttpVerb.PUT);
        captured.Expires.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(5), TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void CreateDownloadUrl_WithExpirationOutsideAllowedRange_ShouldThrow()
    {
        var storage = CreateStorage();

        var act = () => storage.CreateDownloadUrl("person/file.pdf", "exam.pdf", TimeSpan.FromHours(2));

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CreateDownloadUrl_ShouldUseGetExpectedObjectAndSafeContentDisposition()
    {
        GetPreSignedUrlRequest? captured = null;
        _client.GetPreSignedURL(Arg.Do<GetPreSignedUrlRequest>(request => captured = request))
            .Returns("https://example.test/download");
        var storage = CreateStorage();

        var result = storage.CreateDownloadUrl("person/event/file", "../exam result.pdf", TimeSpan.FromMinutes(5));

        result.Should().Be(new Uri("https://example.test/download"));
        captured.Should().NotBeNull();
        captured!.BucketName.Should().Be("private-bucket");
        captured.Key.Should().Be("person/event/file");
        captured.Verb.Should().Be(HttpVerb.GET);
        captured.ResponseHeaderOverrides.ContentDisposition.Should().Be("attachment; filename*=UTF-8''exam%20result.pdf");
        captured.Expires.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(5), TimeSpan.FromSeconds(2));
    }

    private R2ObjectStorage CreateStorage() => new(_client, Options.Create(new R2StorageOptions
    {
        Endpoint = "https://account.r2.cloudflarestorage.com",
        AccountId = "account",
        Bucket = "private-bucket",
        AccessKey = "access-key",
        SecretKey = "secret-key",
        MinPresignedUrlExpirationSeconds = 60,
        MaxPresignedUrlExpirationSeconds = 900
    }));
}
