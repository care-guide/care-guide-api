using CareGuide.Models.Configuration;

namespace CareGuide.Tests.Configuration;

public class FileDownloadOptionsValidatorTests
{
    [Theory]
    [InlineData(59)]
    [InlineData(3601)]
    public void Validate_WithUnsafeExpiration_ShouldFail(int seconds)
    {
        var result = new FileDownloadOptionsValidator().Validate(
            null,
            new FileDownloadOptions { PresignedUrlExpirationSeconds = seconds });

        result.Failed.Should().BeTrue();
    }
}
