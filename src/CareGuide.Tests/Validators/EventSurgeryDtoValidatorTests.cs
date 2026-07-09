using CareGuide.Models.DTOs.EventSurgery;
using CareGuide.Models.Validators.EventSurgery;

namespace CareGuide.Tests.Validators;

public class EventSurgeryDtoValidatorTests
{
    private readonly CreateEventSurgeryDtoValidator _createValidator = new();
    private readonly UpdateEventSurgeryDtoValidator _updateValidator = new();

    // ── CreateEventSurgeryDto ─────────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateEventSurgeryDto("Appendectomy");
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: null Type passes validation")]
    public void Create_NullType_Passes()
    {
        var dto = new CreateEventSurgeryDto(null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: Type exceeding 255 chars fails validation")]
    public void Create_TypeTooLong_Fails()
    {
        var dto = new CreateEventSurgeryDto(new string('a', 256));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    // ── UpdateEventSurgeryDto ─────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventSurgeryDto("Appendectomy");
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: null Type passes validation")]
    public void Update_NullType_Passes()
    {
        var dto = new UpdateEventSurgeryDto(null);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: Type exceeding 255 chars fails validation")]
    public void Update_TypeTooLong_Fails()
    {
        var dto = new UpdateEventSurgeryDto(new string('a', 256));
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }
}
