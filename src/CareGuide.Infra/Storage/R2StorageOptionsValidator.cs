using Microsoft.Extensions.Options;

namespace CareGuide.Infra.Storage;

public sealed class R2StorageOptionsValidator : IValidateOptions<R2StorageOptions>
{
    public ValidateOptionsResult Validate(string? name, R2StorageOptions options)
    {
        var errors = new List<string>();

        if (!Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var endpoint) || endpoint.Scheme != Uri.UriSchemeHttps)
            errors.Add("Storage:R2:Endpoint must be a valid HTTPS URL.");
        if (string.IsNullOrWhiteSpace(options.AccountId)) errors.Add("Storage:R2:AccountId is required.");
        if (string.IsNullOrWhiteSpace(options.Bucket)) errors.Add("Storage:R2:Bucket is required.");
        if (string.IsNullOrWhiteSpace(options.AccessKey)) errors.Add("Storage:R2:AccessKey is required.");
        if (string.IsNullOrWhiteSpace(options.SecretKey)) errors.Add("Storage:R2:SecretKey is required.");
        if (options.MinPresignedUrlExpirationSeconds <= 0)
            errors.Add("Storage:R2:MinPresignedUrlExpirationSeconds must be positive.");
        if (options.MaxPresignedUrlExpirationSeconds < options.MinPresignedUrlExpirationSeconds)
            errors.Add("Storage:R2:MaxPresignedUrlExpirationSeconds must be greater than or equal to the minimum.");

        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}
