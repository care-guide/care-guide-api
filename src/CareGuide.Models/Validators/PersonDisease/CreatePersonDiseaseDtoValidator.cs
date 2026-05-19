using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.PersonDisease;
using FluentValidation;

namespace CareGuide.Models.Validators.PersonDisease
{
    public class CreatePersonDiseaseDtoValidator : AbstractValidator<CreatePersonDiseaseDto>
    {
        public CreatePersonDiseaseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Name must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.DiagnosisDate)
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .When(x => x.DiagnosisDate.HasValue)
                .WithMessage("DiagnosisDate cannot be in the future.");

            RuleFor(x => x.DiseaseType)
                .IsInEnum().WithMessage("DiseaseType is invalid.");
        }
    }
}