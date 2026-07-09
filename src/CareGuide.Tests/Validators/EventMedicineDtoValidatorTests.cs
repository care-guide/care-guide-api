using CareGuide.Models.DTOs.EventMedicine;
using CareGuide.Models.Validators.EventMedicine;

namespace CareGuide.Tests.Validators;

public class EventMedicineDtoValidatorTests
{
    private readonly CreateEventMedicineDtoValidator _createValidator = new();
    private readonly UpdateEventMedicineDtoValidator _updateValidator = new();

    // ── CreateEventMedicineDto ────────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateEventMedicineDto("500mg", "Acme");
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: null Dosage and Laboratory passes validation")]
    public void Create_NullFields_Passes()
    {
        var dto = new CreateEventMedicineDto(null, null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Create: Dosage exceeding 255 chars fails validation")]
    public void Create_DosageTooLong_Fails()
    {
        var dto = new CreateEventMedicineDto(new string('a', 256), null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Dosage");
    }

    [Fact(DisplayName = "Create: Laboratory exceeding 255 chars fails validation")]
    public void Create_LaboratoryTooLong_Fails()
    {
        var dto = new CreateEventMedicineDto(null, new string('a', 256));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Laboratory");
    }

    // ── UpdateEventMedicineDto ────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateEventMedicineDto("500mg", "Acme");
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: null Dosage and Laboratory passes validation")]
    public void Update_NullFields_Passes()
    {
        var dto = new UpdateEventMedicineDto(null, null);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: Dosage exceeding 255 chars fails validation")]
    public void Update_DosageTooLong_Fails()
    {
        var dto = new UpdateEventMedicineDto(new string('a', 256), null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Dosage");
    }

    [Fact(DisplayName = "Update: Laboratory exceeding 255 chars fails validation")]
    public void Update_LaboratoryTooLong_Fails()
    {
        var dto = new UpdateEventMedicineDto(null, new string('a', 256));
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Laboratory");
    }
}
