using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Doctor;
using CareGuide.Models.Validators.Doctor;

namespace CareGuide.Tests.Validators;

public class CreateDoctorDtoValidatorTests
{
    private readonly CreateDoctorDtoValidator _validator = new();

    [Fact(DisplayName = "Valid DTO passes validation")]
    public void ValidDto_PassesValidation()
    {
        var dto = new CreateDoctorDto("Dr. House", "Diagnostics specialist");

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Valid DTO without details passes validation")]
    public void ValidDtoWithoutDetails_PassesValidation()
    {
        var dto = new CreateDoctorDto("Dr. House", null);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyName_FailsWithRequiredMessage(string name)
    {
        var dto = new CreateDoctorDto(name, null);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
    }

    [Fact(DisplayName = "Name exceeding max length fails validation")]
    public void NameTooLong_FailsValidation()
    {
        var dto = new CreateDoctorDto(new string('A', DatabaseConstants.MaxLengthStandardText + 1), null);

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Details exceeding max length fails validation")]
    public void DetailsTooLong_FailsValidation()
    {
        var dto = new CreateDoctorDto("Dr. House", new string('A', DatabaseConstants.MaxLengthLargeText + 1));

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact(DisplayName = "Empty string details skips length validation")]
    public void EmptyStringDetails_SkipsLengthValidation()
    {
        var dto = new CreateDoctorDto("Dr. House", "");

        var result = _validator.Validate(dto);

        result.Errors.Should().NotContain(e => e.PropertyName == "Details");
    }
}
