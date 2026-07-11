using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventAppointment;
using FluentValidation;

namespace CareGuide.Models.Validators.EventAppointment
{
    public class CreateEventAppointmentDtoValidator : AbstractValidator<CreateEventAppointmentDto>
    {
        public CreateEventAppointmentDtoValidator()
        {
            RuleFor(x => x.Doctor)
                .MaximumLength(DatabaseConstants.MaxLengthStandardText)
                .WithMessage($"Doctor must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Doctor));

            RuleFor(x => x.DoctorId)
                .NotEqual(Guid.Empty).WithMessage("DoctorId must not be an empty GUID.")
                .When(x => x.DoctorId.HasValue);

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type is invalid.");
        }
    }
}
