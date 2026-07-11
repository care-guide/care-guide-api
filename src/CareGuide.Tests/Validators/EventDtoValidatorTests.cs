using CareGuide.Models.DTOs.Event;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.Event;

namespace CareGuide.Tests.Validators;

public class EventDtoValidatorTests
{
    private readonly CreateEventDtoValidator _createValidator = new();
    private readonly UpdateEventDtoValidator _updateValidator = new();

    private static readonly DateTime StartDate = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateEventDto("Checkup", StartDate, null, EventType.Appointment, false, null, null, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_Fails(string name)
    {
        var dto = new CreateEventDto(name, StartDate, null, EventType.Appointment, false, null, null, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Create: name exceeding 255 chars fails validation")]
    public void Create_NameTooLong_Fails()
    {
        var dto = new CreateEventDto(new string('a', 256), StartDate, null, EventType.Appointment, false, null, null, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Create: EndDate earlier than StartDate fails validation")]
    public void Create_EndDateBeforeStartDate_Fails()
    {
        var dto = new CreateEventDto("Checkup", StartDate, StartDate.AddDays(-1), EventType.Appointment, false, null, null, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EndDate");
    }

    [Fact(DisplayName = "Create: EndDate equal to StartDate passes validation")]
    public void Create_EndDateEqualStartDate_Passes()
    {
        var dto = new CreateEventDto("Checkup", StartDate, StartDate, EventType.Appointment, false, null, null, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: valid recurrence passes validation")]
    public void Create_ValidRecurrence_Passes()
    {
        var dto = new CreateEventDto("Checkup", StartDate, null, EventType.Appointment, false, RecurrenceType.Appointment, RecurrenceFrequency.Weekly, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: invalid RecurrenceFrequency fails validation")]
    public void Create_InvalidRecurrenceFrequency_Fails()
    {
        var dto = new CreateEventDto("Checkup", StartDate, null, EventType.Appointment, false, RecurrenceType.Appointment, (RecurrenceFrequency)999, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RecurrenceFrequency");
    }

    [Fact(DisplayName = "Create: invalid RecurrenceType fails validation")]
    public void Create_InvalidRecurrenceType_Fails()
    {
        var dto = new CreateEventDto("Checkup", StartDate, null, EventType.Appointment, false, (RecurrenceType)999, RecurrenceFrequency.Weekly, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RecurrenceType");
    }

    [Fact(DisplayName = "Create: Details exceeding 1000 chars fails validation")]
    public void Create_DetailsTooLong_Fails()
    {
        var dto = new CreateEventDto("Checkup", StartDate, null, EventType.Appointment, false, null, null, new string('a', 1001));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventDto(Guid.NewGuid(), "Checkup", StartDate, null, EventType.Appointment, false, null, null, null);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdateEventDto(Guid.Empty, "Checkup", StartDate, null, EventType.Appointment, false, null, null, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_EmptyName_Fails(string name)
    {
        var dto = new UpdateEventDto(Guid.NewGuid(), name, StartDate, null, EventType.Appointment, false, null, null, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Update: EndDate earlier than StartDate fails validation")]
    public void Update_EndDateBeforeStartDate_Fails()
    {
        var dto = new UpdateEventDto(Guid.NewGuid(), "Checkup", StartDate, StartDate.AddDays(-1), EventType.Appointment, false, null, null, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "EndDate");
    }

    [Fact(DisplayName = "Update: invalid RecurrenceFrequency fails validation")]
    public void Update_InvalidRecurrenceFrequency_Fails()
    {
        var dto = new UpdateEventDto(Guid.NewGuid(), "Checkup", StartDate, null, EventType.Appointment, false, RecurrenceType.Surgery, (RecurrenceFrequency)999, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RecurrenceFrequency");
    }
}
