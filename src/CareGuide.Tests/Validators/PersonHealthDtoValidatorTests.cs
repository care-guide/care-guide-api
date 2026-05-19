using CareGuide.Models.DTOs.PersonHealth;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.PersonHealth;

namespace CareGuide.Tests.Validators;

public class PersonHealthDtoValidatorTests
{
    private readonly CreatePersonHealthDtoValidator _createValidator = new();
    private readonly UpdatePersonHealthDtoValidator _updateValidator = new();

    // ── CreatePersonHealthDto ─────────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreatePersonHealthDto(BloodType.A_Positive, 1.75m, 70m, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: invalid blood type fails validation")]
    public void Create_InvalidBloodType_Fails()
    {
        var dto = new CreatePersonHealthDto((BloodType)999, 1.75m, 70m, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BloodType");
    }

    [Theory(DisplayName = "Create: height out of range fails validation")]
    [InlineData(0)]
    [InlineData(-0.1)]
    [InlineData(3.01)]
    public void Create_HeightOutOfRange_Fails(double height)
    {
        var dto = new CreatePersonHealthDto(BloodType.A_Positive, (decimal)height, 70m, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Height");
    }

    [Theory(DisplayName = "Create: height boundary values pass validation")]
    [InlineData(0.01)]
    [InlineData(3.0)]
    public void Create_HeightBoundary_Passes(double height)
    {
        var dto = new CreatePersonHealthDto(BloodType.A_Positive, (decimal)height, 70m, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: weight out of range fails validation")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(500.01)]
    public void Create_WeightOutOfRange_Fails(double weight)
    {
        var dto = new CreatePersonHealthDto(BloodType.A_Positive, 1.75m, (decimal)weight, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Weight");
    }

    [Theory(DisplayName = "Create: weight boundary values pass validation")]
    [InlineData(0.01)]
    [InlineData(500)]
    public void Create_WeightBoundary_Passes(double weight)
    {
        var dto = new CreatePersonHealthDto(BloodType.A_Positive, 1.75m, (decimal)weight, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: description exceeding 1000 chars fails validation")]
    public void Create_DescriptionTooLong_Fails()
    {
        var dto = new CreatePersonHealthDto(BloodType.A_Positive, 1.75m, 70m, new string('a', 1001));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }

    [Fact(DisplayName = "Create: null or empty description skips length validation")]
    public void Create_NullDescription_Passes()
    {
        var dto = new CreatePersonHealthDto(BloodType.A_Positive, 1.75m, 70m, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    // ── UpdatePersonHealthDto ─────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdatePersonHealthDto(Guid.NewGuid(), BloodType.O_Positive, 1.80m, 80m, null);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdatePersonHealthDto(Guid.Empty, BloodType.O_Positive, 1.80m, 80m, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: height out of range fails validation")]
    [InlineData(0)]
    [InlineData(3.01)]
    public void Update_HeightOutOfRange_Fails(double height)
    {
        var dto = new UpdatePersonHealthDto(Guid.NewGuid(), BloodType.O_Positive, (decimal)height, 80m, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Height");
    }

    [Theory(DisplayName = "Update: weight out of range fails validation")]
    [InlineData(0)]
    [InlineData(500.01)]
    public void Update_WeightOutOfRange_Fails(double weight)
    {
        var dto = new UpdatePersonHealthDto(Guid.NewGuid(), BloodType.O_Positive, 1.75m, (decimal)weight, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Weight");
    }
}
