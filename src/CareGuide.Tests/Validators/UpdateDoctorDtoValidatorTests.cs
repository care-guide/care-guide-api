using CareGuide.Models.DTOs.Doctor;
using CareGuide.Models.Validators.Doctor;

namespace CareGuide.Tests.Validators;

public class UpdateDoctorDtoValidatorTests
{
    private readonly UpdateDoctorDtoValidator _validator = new();

    [Fact(DisplayName = "Valid dto passes validation")]
    public void Validate_ValidDto_Passes()
    {
        var dto = new UpdateDoctorDto(Guid.NewGuid(), "Dr. House", "Diagnostics specialist");
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Valid dto without details passes validation")]
    public void Validate_ValidDto_NoDetails_Passes()
    {
        var dto = new UpdateDoctorDto(Guid.NewGuid(), "Dr. House", null);
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Empty Id fails validation")]
    public void Validate_EmptyId_Fails()
    {
        var dto = new UpdateDoctorDto(Guid.Empty, "Dr. House", null);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyName_Fails(string name)
    {
        var dto = new UpdateDoctorDto(Guid.NewGuid(), name, null);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Name exceeding 255 chars fails validation")]
    public void Validate_NameTooLong_Fails()
    {
        var dto = new UpdateDoctorDto(Guid.NewGuid(), new string('a', 256), null);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Details exceeding 1000 chars fails validation")]
    public void Validate_DetailsTooLong_Fails()
    {
        var dto = new UpdateDoctorDto(Guid.NewGuid(), "Dr. House", new string('a', 1001));
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact(DisplayName = "Empty string details skips length validation")]
    public void Validate_EmptyStringDetails_Passes()
    {
        var dto = new UpdateDoctorDto(Guid.NewGuid(), "Dr. House", "");
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }
}
