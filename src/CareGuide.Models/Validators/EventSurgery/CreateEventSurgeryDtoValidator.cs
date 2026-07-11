using CareGuide.Models.DTOs.EventSurgery;
using FluentValidation;

namespace CareGuide.Models.Validators.EventSurgery
{
    public class CreateEventSurgeryDtoValidator : AbstractValidator<CreateEventSurgeryDto>
    {
        public CreateEventSurgeryDtoValidator()
        {
            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type is invalid.");
        }
    }
}
