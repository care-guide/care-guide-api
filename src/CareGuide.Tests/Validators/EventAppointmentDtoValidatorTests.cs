using CareGuide.Models.DTOs.EventAppointment;
using CareGuide.Models.Validators.EventAppointment;

namespace CareGuide.Tests.Validators;

public class EventAppointmentDtoValidatorTests
{
    private readonly CreateEventAppointmentDtoValidator _createValidator = new();
    private readonly UpdateEventAppointmentDtoValidator _updateValidator = new();

    // ── CreateEventAppointmentDto ────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateEventAppointmentDto("Dr. House", "Checkup");
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: null Doctor and Type passes validation")]
    public void Create_NullFields_Passes()
    {
        var dto = new CreateEventAppointmentDto(null, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: Doctor exceeding 255 chars fails validation")]
    public void Create_DoctorTooLong_Fails()
    {
        var dto = new CreateEventAppointmentDto(new string('a', 256), null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Doctor");
    }

    [Fact(DisplayName = "Create: Type exceeding 255 chars fails validation")]
    public void Create_TypeTooLong_Fails()
    {
        var dto = new CreateEventAppointmentDto(null, new string('a', 256));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    // ── UpdateEventAppointmentDto ─────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventAppointmentDto("Dr. House", "Checkup");
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: null Doctor and Type passes validation")]
    public void Update_NullFields_Passes()
    {
        var dto = new UpdateEventAppointmentDto(null, null);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: Doctor exceeding 255 chars fails validation")]
    public void Update_DoctorTooLong_Fails()
    {
        var dto = new UpdateEventAppointmentDto(new string('a', 256), null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Doctor");
    }

    [Fact(DisplayName = "Update: Type exceeding 255 chars fails validation")]
    public void Update_TypeTooLong_Fails()
    {
        var dto = new UpdateEventAppointmentDto(null, new string('a', 256));
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }
}
