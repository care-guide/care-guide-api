using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.FileAsset;
using FluentValidation;

namespace CareGuide.Models.Validators.FileAsset
{
    public class UpdateFileAssetDtoValidator : AbstractValidator<UpdateFileAssetDto>
    {
        public UpdateFileAssetDtoValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.")
                .NotEqual(Guid.Empty).WithMessage("Id cannot be an empty Guid.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Name must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.Size)
                .GreaterThan(0).WithMessage("Size must be greater than 0.");

            RuleFor(x => x.Hash)
                .NotEmpty().WithMessage("Hash is required.")
                .Matches(@"^[a-fA-F0-9]{32,128}$").WithMessage("Hash must be a hexadecimal digest between 32 and 128 characters long.");
        }
    }
}
