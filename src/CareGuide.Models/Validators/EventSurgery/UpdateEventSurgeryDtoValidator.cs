using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventSurgery;
using FluentValidation;

namespace CareGuide.Models.Validators.EventSurgery
{
    public class UpdateEventSurgeryDtoValidator : AbstractValidator<UpdateEventSurgeryDto>
    {
        public UpdateEventSurgeryDtoValidator()
        {
            RuleFor(x => x.Type)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText)
                .WithMessage($"Type must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Type));
        }
    }
}
