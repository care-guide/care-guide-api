using CareGuide.Models.DTOs.EventExam;
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
        var dto = new CreateEventExamDto("Blood Test");
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: null Type passes validation")]
    public void Create_NullFields_Passes()
    {
        var dto = new CreateEventExamDto(null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: Type exceeding 255 chars fails validation")]
    public void Create_TypeTooLong_Fails()
    {
        var dto = new CreateEventExamDto(new string('a', 256));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    // ── UpdateEventExamDto ────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventExamDto("Blood Test");
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: null Type passes validation")]
    public void Update_NullFields_Passes()
    {
        var dto = new UpdateEventExamDto(null);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: Type exceeding 255 chars fails validation")]
    public void Update_TypeTooLong_Fails()
    {
        var dto = new UpdateEventExamDto(new string('a', 256));
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }
}
