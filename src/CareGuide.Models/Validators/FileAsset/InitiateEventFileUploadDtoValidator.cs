using CareGuide.Models.Configuration;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.FileAsset;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace CareGuide.Models.Validators.FileAsset;

public sealed class InitiateEventFileUploadDtoValidator : AbstractValidator<InitiateEventFileUploadDto>
{
    public InitiateEventFileUploadDtoValidator(IOptions<FileUploadOptions> options)
    {
        var settings = options.Value;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(DatabaseConstants.MaxLengthStandardText)
            .Must(name => settings.AllowedExtensions.Contains(Path.GetExtension(name), StringComparer.OrdinalIgnoreCase))
            .WithMessage("File extension is not allowed.");

        RuleFor(x => x.Size)
            .GreaterThan(0)
            .LessThanOrEqualTo(settings.MaxSizeBytes);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(contentType => settings.AllowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Content type is not allowed.");

        RuleFor(x => x.ExpectedHash)
            .NotEmpty()
            .Matches("^[a-fA-F0-9]{64}$")
            .WithMessage($"ExpectedHash must be a {settings.HashAlgorithm} digest in {settings.HashFormat} format.");
    }
}
