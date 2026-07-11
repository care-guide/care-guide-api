using Microsoft.Extensions.Options;

namespace CareGuide.Models.Configuration;

public sealed class FileLifecycleOptionsValidator : IValidateOptions<FileLifecycleOptions>
{
    public ValidateOptionsResult Validate(string? name, FileLifecycleOptions options)
    {
        var errors = new List<string>();
        if (options.PendingUploadExpirationMinutes <= 0) errors.Add("FileLifecycle:PendingUploadExpirationMinutes must be positive.");
        if (options.UnreferencedFileRetentionDays < 0) errors.Add("FileLifecycle:UnreferencedFileRetentionDays cannot be negative.");
        if (options.CleanupIntervalMinutes <= 0) errors.Add("FileLifecycle:CleanupIntervalMinutes must be positive.");
        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}
