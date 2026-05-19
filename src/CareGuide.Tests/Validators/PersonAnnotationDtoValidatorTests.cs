using CareGuide.Models.DTOs.PersonAnnotation;
using CareGuide.Models.Validators.PersonAnnotation;

namespace CareGuide.Tests.Validators;

public class PersonAnnotationDtoValidatorTests
{
    private readonly CreatePersonAnnotationDtoValidator _createValidator = new();
    private readonly UpdatePersonAnnotationDtoValidator _updateValidator = new();

    // ── CreatePersonAnnotationDto ─────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreatePersonAnnotationDto("Annual checkup notes", null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace details fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyDetails_Fails(string details)
    {
        var dto = new CreatePersonAnnotationDto(details, null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact(DisplayName = "Create: details exceeding 1000 chars fails validation")]
    public void Create_DetailsTooLong_Fails()
    {
        var dto = new CreatePersonAnnotationDto(new string('a', 1001), null);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact(DisplayName = "Create: fileUrl exceeding 255 chars fails validation")]
    public void Create_FileUrlTooLong_Fails()
    {
        var dto = new CreatePersonAnnotationDto("Details", new string('a', 256));
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FileUrl");
    }

    [Fact(DisplayName = "Create: null fileUrl passes validation")]
    public void Create_NullFileUrl_Passes()
    {
        var dto = new CreatePersonAnnotationDto("Details", null);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    // ── UpdatePersonAnnotationDto ─────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdatePersonAnnotationDto(Guid.NewGuid(), "Updated notes", null);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdatePersonAnnotationDto(Guid.Empty, "Details", null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: empty or whitespace details fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_EmptyDetails_Fails(string details)
    {
        var dto = new UpdatePersonAnnotationDto(Guid.NewGuid(), details, null);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Details");
    }

    [Fact(DisplayName = "Update: fileUrl exceeding 255 chars fails validation")]
    public void Update_FileUrlTooLong_Fails()
    {
        var dto = new UpdatePersonAnnotationDto(Guid.NewGuid(), "Details", new string('a', 256));
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "FileUrl");
    }
}
