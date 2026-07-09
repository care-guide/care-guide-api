using CareGuide.Models.DTOs.DoctorAddress;
using CareGuide.Models.Validators.DoctorAddress;

namespace CareGuide.Tests.Validators;

public class DoctorAddressDtoValidatorTests
{
    private readonly CreateDoctorAddressDtoValidator _createValidator = new();

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateDoctorAddressDto(Guid.NewGuid());

        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: empty AddressId fails validation")]
    public void Create_EmptyAddressId_Fails()
    {
        var dto = new CreateDoctorAddressDto(Guid.Empty);

        var result = _createValidator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AddressId");
    }
}
