using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Account;
using FluentValidation;

namespace CareGuide.Models.Validators.Account
{
    public class LoginAccountDtoValidator : AbstractValidator<LoginAccountDto>
    {
        public LoginAccountDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Email must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Password must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");
        }
    }
}
