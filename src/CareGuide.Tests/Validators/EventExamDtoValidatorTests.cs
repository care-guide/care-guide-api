using CareGuide.Models.DTOs.EventExam;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.EventExam;

namespace CareGuide.Tests.Validators;

public class EventExamDtoValidatorTests
{
    private readonly CreateEventExamDtoValidator _createValidator = new();
    private readonly UpdateEventExamDtoValidator _updateValidator = new();

    // ── CreateEventExamDto ────────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateEventExamDto(EventExamType.Blood);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: invalid Type fails validation")]
    public void Create_InvalidType_Fails()
    {
        var dto = new CreateEventExamDto((EventExamType)999);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    // ── UpdateEventExamDto ────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventExamDto(EventExamType.Imaging);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: invalid Type fails validation")]
    public void Update_InvalidType_Fails()
    {
        var dto = new UpdateEventExamDto((EventExamType)999);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }
}
