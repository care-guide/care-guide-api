using CareGuide.Infra.Storage;

namespace CareGuide.Tests.Storage;

public class R2StorageOptionsValidatorTests
{
    [Fact]
    public void Validate_WithValidOptions_ShouldSucceed()
    {
        var result = new R2StorageOptionsValidator().Validate(null, ValidOptions());

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithMissingConfiguration_ShouldFailWithoutExposingSecrets()
    {
        var result = new R2StorageOptionsValidator().Validate(null, new R2StorageOptions());

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(message => message.Contains("Endpoint"));
        result.Failures.Should().Contain(message => message.Contains("SecretKey"));
        result.Failures.Should().NotContain(message => message.Contains("secret-value"));
    }

    private static R2StorageOptions ValidOptions() => new()
    {
        Endpoint = "https://account.r2.cloudflarestorage.com",
        AccountId = "account",
        Bucket = "private-bucket",
        AccessKey = "access-key",
        SecretKey = "secret-value",
        MinPresignedUrlExpirationSeconds = 60,
        MaxPresignedUrlExpirationSeconds = 900
    };
}
