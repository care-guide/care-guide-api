using CareGuide.Models.DTOs.EventExam;
using FluentValidation;

namespace CareGuide.Models.Validators.EventExam
{
    public class CreateEventExamDtoValidator : AbstractValidator<CreateEventExamDto>
    {
        public CreateEventExamDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type is invalid.");
        }
    }
}
