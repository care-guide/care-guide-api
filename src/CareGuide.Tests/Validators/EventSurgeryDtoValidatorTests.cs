using CareGuide.Models.DTOs.EventSurgery;
using CareGuide.Models.Enums;
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
        var dto = new CreateEventSurgeryDto(EventSurgeryType.Curative);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: invalid Type fails validation")]
    public void Create_InvalidType_Fails()
    {
        var dto = new CreateEventSurgeryDto((EventSurgeryType)999);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    // ── UpdateEventSurgeryDto ─────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventSurgeryDto(EventSurgeryType.Palliative);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: invalid Type fails validation")]
    public void Update_InvalidType_Fails()
    {
        var dto = new UpdateEventSurgeryDto((EventSurgeryType)999);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }
}
