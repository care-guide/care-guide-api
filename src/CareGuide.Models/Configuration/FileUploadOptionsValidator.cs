using Microsoft.Extensions.Options;

namespace CareGuide.Models.Configuration;

public sealed class FileUploadOptionsValidator : IValidateOptions<FileUploadOptions>
{
    public ValidateOptionsResult Validate(string? name, FileUploadOptions options)
    {
        var errors = new List<string>();
        if (options.MaxSizeBytes <= 0) errors.Add("FileUploads:MaxSizeBytes must be positive.");
        if (options.AllowedContentTypes.Length == 0) errors.Add("FileUploads:AllowedContentTypes must not be empty.");
        if (options.AllowedExtensions.Length == 0) errors.Add("FileUploads:AllowedExtensions must not be empty.");
        if (!string.Equals(options.HashAlgorithm, "SHA256", StringComparison.OrdinalIgnoreCase))
            errors.Add("FileUploads:HashAlgorithm must be SHA256.");
        if (!string.Equals(options.HashFormat, "Hex", StringComparison.OrdinalIgnoreCase))
            errors.Add("FileUploads:HashFormat must be Hex.");
        if (options.PresignedUrlExpirationSeconds <= 0)
            errors.Add("FileUploads:PresignedUrlExpirationSeconds must be positive.");

        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}
