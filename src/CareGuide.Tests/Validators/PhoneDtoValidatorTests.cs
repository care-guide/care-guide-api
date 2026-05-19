using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.Phone;

namespace CareGuide.Tests.Validators;

public class PhoneDtoValidatorTests
{
    private readonly CreatePhoneDtoValidator _createValidator = new();
    private readonly UpdatePhoneDtoValidator _updateValidator = new();

    // ── CreatePhoneDto ────────────────────────────────────────────────────────

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreatePhoneDto("912345678", "11", PhoneType.CEL);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace number fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyNumber_Fails(string number)
    {
        var dto = new CreatePhoneDto(number, "11", PhoneType.CEL);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Number");
    }

    [Theory(DisplayName = "Create: number with non-digits or wrong length fails validation")]
    [InlineData("1234567")]
    [InlineData("1234567890123")]
    [InlineData("9123abc78")]
    public void Create_InvalidNumberFormat_Fails(string number)
    {
        var dto = new CreatePhoneDto(number, "11", PhoneType.CEL);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Number");
    }

    [Theory(DisplayName = "Create: number boundary lengths 8 and 12 digits pass validation")]
    [InlineData("12345678")]
    [InlineData("123456789012")]
    public void Create_NumberBoundaryLength_Passes(string number)
    {
        var dto = new CreatePhoneDto(number, "11", PhoneType.CEL);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace area code fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyAreaCode_Fails(string areaCode)
    {
        var dto = new CreatePhoneDto("912345678", areaCode, PhoneType.CEL);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AreaCode");
    }

    [Theory(DisplayName = "Create: area code with non-digits or wrong length fails validation")]
    [InlineData("1")]
    [InlineData("123456")]
    [InlineData("1a")]
    public void Create_InvalidAreaCodeFormat_Fails(string areaCode)
    {
        var dto = new CreatePhoneDto("912345678", areaCode, PhoneType.CEL);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AreaCode");
    }

    [Fact(DisplayName = "Create: invalid phone type fails validation")]
    public void Create_InvalidPhoneType_Fails()
    {
        var dto = new CreatePhoneDto("912345678", "11", (PhoneType)999);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    // ── UpdatePhoneDto ────────────────────────────────────────────────────────

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdatePhoneDto(Guid.NewGuid(), "987654321", "21", PhoneType.R);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdatePhoneDto(Guid.Empty, "912345678", "11", PhoneType.CEL);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: number with non-digits or wrong length fails validation")]
    [InlineData("1234567")]
    [InlineData("1234567890123")]
    [InlineData("abc12345")]
    public void Update_InvalidNumberFormat_Fails(string number)
    {
        var dto = new UpdatePhoneDto(Guid.NewGuid(), number, "11", PhoneType.CEL);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Number");
    }

    [Theory(DisplayName = "Update: area code with non-digits or wrong length fails validation")]
    [InlineData("1")]
    [InlineData("123456")]
    public void Update_InvalidAreaCodeFormat_Fails(string areaCode)
    {
        var dto = new UpdatePhoneDto(Guid.NewGuid(), "912345678", areaCode, PhoneType.CEL);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AreaCode");
    }
}
