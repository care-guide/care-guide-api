using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Person;
using FluentValidation;

namespace CareGuide.Models.Validators.Person
{
    public class UpdatePersonProfileDtoValidator : AbstractValidator<UpdatePersonProfileDto>
    {
        public UpdatePersonProfileDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Name must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Gender is invalid.");

            RuleFor(x => x.Birthday)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow)).WithMessage("Birthday cannot be in the future.");

            RuleFor(x => x.Picture)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText)
                .When(x => !string.IsNullOrWhiteSpace(x.Picture))
                .WithMessage($"Picture must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");
        }
    }
}
