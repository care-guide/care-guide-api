using CareGuide.Models.DTOs.PersonFamilyHistory;
using CareGuide.Models.Validators.PersonFamilyHistory;

namespace CareGuide.Tests.Validators;

public class PersonFamilyHistoryDtoValidatorTests
{
    private readonly CreatePersonFamilyHistoryDtoValidator _createValidator = new();
    private readonly UpdatePersonFamilyHistoryDtoValidator _updateValidator = new();

    // ── CreatePersonFamilyHistoryDto ──────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreatePersonFamilyHistoryDto("Father", "Heart disease", 55);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: null age passes validation")]
    public void Create_NullAge_Passes()
    {
        var dto = new CreatePersonFamilyHistoryDto("Mother", "Cancer", null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace relationship fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyRelationship_Fails(string relationship)
    {
        var dto = new CreatePersonFamilyHistoryDto(relationship, "Diabetes", null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Relationship");
    }

    [Theory(DisplayName = "Create: empty or whitespace diagnosis fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyDiagnosis_Fails(string diagnosis)
    {
        var dto = new CreatePersonFamilyHistoryDto("Brother", diagnosis, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Diagnosis");
    }

    [Theory(DisplayName = "Create: age out of 0-150 range fails validation")]
    [InlineData(-1)]
    [InlineData(151)]
    public void Create_AgeOutOfRange_Fails(int age)
    {
        var dto = new CreatePersonFamilyHistoryDto("Father", "Diabetes", age);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AgeAtDiagnosis");
    }

    [Theory(DisplayName = "Create: boundary ages 0 and 150 pass validation")]
    [InlineData(0)]
    [InlineData(150)]
    public void Create_BoundaryAge_Passes(int age)
    {
        var dto = new CreatePersonFamilyHistoryDto("Father", "Diabetes", age);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    // ── UpdatePersonFamilyHistoryDto ──────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdatePersonFamilyHistoryDto(Guid.NewGuid(), "Father", "Heart disease", 55);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdatePersonFamilyHistoryDto(Guid.Empty, "Father", "Diabetes", null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: age out of 0-150 range fails validation")]
    [InlineData(-1)]
    [InlineData(151)]
    public void Update_AgeOutOfRange_Fails(int age)
    {
        var dto = new UpdatePersonFamilyHistoryDto(Guid.NewGuid(), "Father", "Diabetes", age);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AgeAtDiagnosis");
    }
}
