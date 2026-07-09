using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventExam;
using FluentValidation;

namespace CareGuide.Models.Validators.EventExam
{
    public class UpdateEventExamDtoValidator : AbstractValidator<UpdateEventExamDto>
    {
        public UpdateEventExamDtoValidator()
        {
            RuleFor(x => x.Type)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText)
                .WithMessage($"Type must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Type));
        }
    }
}
