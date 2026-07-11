using Microsoft.Extensions.Options;

namespace CareGuide.Models.Configuration;

public sealed class FileDownloadOptionsValidator : IValidateOptions<FileDownloadOptions>
{
    public ValidateOptionsResult Validate(string? name, FileDownloadOptions options) =>
        options.PresignedUrlExpirationSeconds is >= 60 and <= 3600
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("FileDownloads:PresignedUrlExpirationSeconds must be between 60 and 3600.");
}
