using CareGuide.Models.Configuration;

namespace CareGuide.Tests.Configuration;

public class FileLifecycleOptionsValidatorTests
{
    [Fact]
    public void Validate_WithInvalidPeriods_ShouldFail()
    {
        var result = new FileLifecycleOptionsValidator().Validate(null, new FileLifecycleOptions
        {
            PendingUploadExpirationMinutes = 0,
            UnreferencedFileRetentionDays = -1,
            CleanupIntervalMinutes = 0
        });

        result.Failed.Should().BeTrue();
        result.Failures.Should().HaveCount(3);
    }
}
