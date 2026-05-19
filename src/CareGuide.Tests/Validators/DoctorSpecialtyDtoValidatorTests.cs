using CareGuide.Models.DTOs.DoctorSpecialty;
using CareGuide.Models.Validators.DoctorSpecialty;

namespace CareGuide.Tests.Validators;

public class DoctorSpecialtyDtoValidatorTests
{
    private readonly CreateDoctorSpecialtyDtoValidator _createValidator = new();
    private readonly UpdateDoctorSpecialtyDtoValidator _updateValidator = new();

    // ── CreateDoctorSpecialtyDto ──────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateDoctorSpecialtyDto("Cardiology");
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_Fails(string name)
    {
        var dto = new CreateDoctorSpecialtyDto(name);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Create: name exceeding 255 chars fails validation")]
    public void Create_NameTooLong_Fails()
    {
        var dto = new CreateDoctorSpecialtyDto(new string('a', 256));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    // ── UpdateDoctorSpecialtyDto ──────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateDoctorSpecialtyDto(Guid.NewGuid(), "Neurology");
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdateDoctorSpecialtyDto(Guid.Empty, "Neurology");
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_EmptyName_Fails(string name)
    {
        var dto = new UpdateDoctorSpecialtyDto(Guid.NewGuid(), name);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Update: name exceeding 255 chars fails validation")]
    public void Update_NameTooLong_Fails()
    {
        var dto = new UpdateDoctorSpecialtyDto(Guid.NewGuid(), new string('a', 256));
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }
}
