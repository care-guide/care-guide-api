using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Account;
using FluentValidation;

namespace CareGuide.Models.Validators.Account
{
    public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
    {
        public CreateAccountDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Email must not exceed {DatabaseConstants.MaxLengthStandardText} characters.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Password must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Name must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage("Gender must be 'M', 'F', or 'O'.");

            RuleFor(x => x.Birthday)
                .NotEmpty().WithMessage("Birthday is required.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Birthday cannot be in the future.");
        }
    }
}
