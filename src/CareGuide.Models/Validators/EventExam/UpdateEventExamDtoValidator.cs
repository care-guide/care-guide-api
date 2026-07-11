using CareGuide.Models.DTOs.EventExam;
using FluentValidation;

namespace CareGuide.Models.Validators.EventExam
{
    public class UpdateEventExamDtoValidator : AbstractValidator<UpdateEventExamDto>
    {
        public UpdateEventExamDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type is invalid.");
        }
    }
}
