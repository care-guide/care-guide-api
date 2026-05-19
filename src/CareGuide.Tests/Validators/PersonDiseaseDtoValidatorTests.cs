using CareGuide.Models.DTOs.PersonDisease;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.PersonDisease;

namespace CareGuide.Tests.Validators;

public class PersonDiseaseDtoValidatorTests
{
    private readonly CreatePersonDiseaseDtoValidator _createValidator = new();
    private readonly UpdatePersonDiseaseDtoValidator _updateValidator = new();

    // ── CreatePersonDiseaseDto ────────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreatePersonDiseaseDto("Hypertension", new DateOnly(2020, 1, 1), DiseaseType.Chronic);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: null diagnosis date passes validation")]
    public void Create_NullDiagnosisDate_Passes()
    {
        var dto = new CreatePersonDiseaseDto("Asthma", null, DiseaseType.Chronic);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_Fails(string name)
    {
        var dto = new CreatePersonDiseaseDto(name, null, DiseaseType.Chronic);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Create: name exceeding 255 chars fails validation")]
    public void Create_NameTooLong_Fails()
    {
        var dto = new CreatePersonDiseaseDto(new string('a', 256), null, DiseaseType.Chronic);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Create: future diagnosis date fails validation")]
    public void Create_FutureDiagnosisDate_Fails()
    {
        var dto = new CreatePersonDiseaseDto("Diabetes", DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DiseaseType.Chronic);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiagnosisDate");
    }

    [Fact(DisplayName = "Create: invalid disease type fails validation")]
    public void Create_InvalidDiseaseType_Fails()
    {
        var dto = new CreatePersonDiseaseDto("Disease", null, (DiseaseType)999);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiseaseType");
    }

    // ── UpdatePersonDiseaseDto ────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdatePersonDiseaseDto(Guid.NewGuid(), "Hypertension", new DateOnly(2020, 1, 1), DiseaseType.Chronic);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdatePersonDiseaseDto(Guid.Empty, "Hypertension", null, DiseaseType.Chronic);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact(DisplayName = "Update: future diagnosis date fails validation")]
    public void Update_FutureDiagnosisDate_Fails()
    {
        var dto = new UpdatePersonDiseaseDto(Guid.NewGuid(), "Diabetes", DateOnly.FromDateTime(DateTime.Now.AddDays(1)), DiseaseType.Chronic);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DiagnosisDate");
    }
}
