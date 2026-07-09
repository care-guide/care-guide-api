using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Address;
using FluentValidation;

namespace CareGuide.Models.Validators.Address
{
    public class CreateAddressDtoValidator : AbstractValidator<CreateAddressDto>
    {
        public CreateAddressDtoValidator()
        {
            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Postal code is required.")
                .Matches(@"^[A-Za-z0-9\-\s]{3,20}$").WithMessage("Postal code must be alphanumeric and between 3 and 20 characters long.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"City must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Country must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State must not exceed 100 characters.");

            RuleFor(x => x.Type)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Type must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Type));

            RuleFor(x => x.Complement)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Complement must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Complement));

            RuleFor(x => x.District)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"District must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.District));

            RuleFor(x => x.ReferencePoint)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Reference point must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.ReferencePoint));

            RuleFor(x => x.Number)
                .GreaterThan(0).WithMessage("Number must be greater than zero.")
                .When(x => x.Number.HasValue);
        }
    }
}
