using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.FileAsset;
using FluentValidation;

namespace CareGuide.Models.Validators.FileAsset
{
    public class CreateFileAssetDtoValidator : AbstractValidator<CreateFileAssetDto>
    {
        public CreateFileAssetDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Name must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.Size)
                .GreaterThan(0).WithMessage("Size must be greater than 0.");

            RuleFor(x => x.Hash)
                .NotEmpty().WithMessage("Hash is required.")
                .Matches(@"^[a-fA-F0-9]{32,128}$").WithMessage("Hash must be a hexadecimal digest between 32 and 128 characters long.");

            RuleFor(x => x.ContentType)
                .NotEmpty().WithMessage("ContentType is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"ContentType must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .Matches(@"^[a-zA-Z0-9][a-zA-Z0-9!#$&^_.+-]*/[a-zA-Z0-9][a-zA-Z0-9!#$&^_.+-]*$").WithMessage("ContentType must be a valid media type.");

        }
    }
}
