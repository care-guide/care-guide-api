using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Account;
using FluentValidation;

namespace CareGuide.Models.Validators.Account
{
    public class UpdatePasswordAccountDtoValidator : AbstractValidator<UpdatePasswordAccountDto>
    {
        public UpdatePasswordAccountDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"Current password must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MaximumLength(DatabaseConstants.MaxLengthStandardText).WithMessage($"New password must not exceed {DatabaseConstants.MaxLengthStandardText} characters.");
        }
    }
}
