using CareGuide.Models.DTOs.EventSurgery;
using FluentValidation;

namespace CareGuide.Models.Validators.EventSurgery
{
    public class UpdateEventSurgeryDtoValidator : AbstractValidator<UpdateEventSurgeryDto>
    {
        public UpdateEventSurgeryDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type is invalid.");
        }
    }
}
