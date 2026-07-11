using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Event;
using FluentValidation;

namespace CareGuide.Models.Validators.Event
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText)
                .WithMessage($"Name must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("EndDate must not be earlier than StartDate.")
                .When(x => x.EndDate.HasValue);

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Type must be a valid event type.");

            RuleFor(x => x.RecurrenceType)
                .IsInEnum().WithMessage("RecurrenceType must be a valid recurrence type.")
                .When(x => x.RecurrenceType.HasValue);

            RuleFor(x => x.RecurrenceFrequency)
                .IsInEnum().WithMessage("RecurrenceFrequency must be a valid recurrence frequency.")
                .When(x => x.RecurrenceFrequency.HasValue);

            RuleFor(x => x.Details)
                .MaximumLength(DatabaseConstants.MaxLengthLargeText)
                .WithMessage($"Details must not exceed {DatabaseConstants.MaxLengthLargeText} characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Details));
        }
    }
}
