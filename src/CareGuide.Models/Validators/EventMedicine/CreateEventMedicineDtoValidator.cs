using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventMedicine;
using FluentValidation;

namespace CareGuide.Models.Validators.EventMedicine
{
    public class CreateEventMedicineDtoValidator : AbstractValidator<CreateEventMedicineDto>
    {
        public CreateEventMedicineDtoValidator()
        {
            RuleFor(x => x.Dosage)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText)
                .WithMessage($"Dosage must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dosage));

            RuleFor(x => x.Laboratory)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText)
                .WithMessage($"Laboratory must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Laboratory));
        }
    }
}
