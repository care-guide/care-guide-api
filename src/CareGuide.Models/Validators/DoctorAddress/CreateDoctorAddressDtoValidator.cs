using CareGuide.Models.DTOs.DoctorAddress;
using FluentValidation;

namespace CareGuide.Models.Validators.DoctorAddress
{
    public class CreateDoctorAddressDtoValidator : AbstractValidator<CreateDoctorAddressDto>
    {
        public CreateDoctorAddressDtoValidator()
        {
            RuleFor(x => x.AddressId)
                .NotEmpty().WithMessage("AddressId is required.")
                .NotEqual(Guid.Empty).WithMessage("AddressId cannot be an empty Guid.");
        }
    }
}
