using CareGuide.Models.Configuration;

namespace CareGuide.Tests.Configuration;

public class FileUploadOptionsValidatorTests
{
    [Fact]
    public void Validate_WithMissingAllowLists_ShouldFail()
    {
        var result = new FileUploadOptionsValidator().Validate(null, new FileUploadOptions());

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(message => message.Contains("AllowedContentTypes"));
        result.Failures.Should().Contain(message => message.Contains("AllowedExtensions"));
    }
}
