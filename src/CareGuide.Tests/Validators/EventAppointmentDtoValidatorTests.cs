using CareGuide.Models.DTOs.EventAppointment;
using CareGuide.Models.Enums;
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
        var dto = new CreateEventAppointmentDto("Dr. House", null, EventAppointmentType.Routine);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: null Doctor passes validation")]
    public void Create_NullDoctor_Passes()
    {
        var dto = new CreateEventAppointmentDto(null, null, EventAppointmentType.Emergency);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: Doctor exceeding 255 chars fails validation")]
    public void Create_DoctorTooLong_Fails()
    {
        var dto = new CreateEventAppointmentDto(new string('a', 256), null, EventAppointmentType.Routine);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Doctor");
    }

    [Fact(DisplayName = "Create: invalid Type fails validation")]
    public void Create_InvalidType_Fails()
    {
        var dto = new CreateEventAppointmentDto("Dr. House", null, (EventAppointmentType)999);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    [Fact(DisplayName = "Create: valid DoctorId passes validation")]
    public void Create_ValidDoctorId_Passes()
    {
        var dto = new CreateEventAppointmentDto(null, Guid.NewGuid(), EventAppointmentType.Routine);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: empty GUID DoctorId fails validation")]
    public void Create_EmptyDoctorId_Fails()
    {
        var dto = new CreateEventAppointmentDto(null, Guid.Empty, EventAppointmentType.Routine);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DoctorId");
    }

    // ── UpdateEventAppointmentDto ─────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventAppointmentDto("Dr. House", null, EventAppointmentType.FollowUp);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: null Doctor passes validation")]
    public void Update_NullDoctor_Passes()
    {
        var dto = new UpdateEventAppointmentDto(null, null, EventAppointmentType.Screening);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: Doctor exceeding 255 chars fails validation")]
    public void Update_DoctorTooLong_Fails()
    {
        var dto = new UpdateEventAppointmentDto(new string('a', 256), null, EventAppointmentType.Routine);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Doctor");
    }

    [Fact(DisplayName = "Update: invalid Type fails validation")]
    public void Update_InvalidType_Fails()
    {
        var dto = new UpdateEventAppointmentDto("Dr. House", null, (EventAppointmentType)999);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    [Fact(DisplayName = "Update: valid DoctorId passes validation")]
    public void Update_ValidDoctorId_Passes()
    {
        var dto = new UpdateEventAppointmentDto(null, Guid.NewGuid(), EventAppointmentType.Routine);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty GUID DoctorId fails validation")]
    public void Update_EmptyDoctorId_Fails()
    {
        var dto = new UpdateEventAppointmentDto(null, Guid.Empty, EventAppointmentType.Routine);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "DoctorId");
    }
}
