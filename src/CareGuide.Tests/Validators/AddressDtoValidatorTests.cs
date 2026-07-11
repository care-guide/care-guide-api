using CareGuide.Models.DTOs.Address;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.Address;

namespace CareGuide.Tests.Validators;

public class AddressDtoValidatorTests
{
    private readonly CreateAddressDtoValidator _createValidator = new();
    private readonly UpdateAddressDtoValidator _updateValidator = new();

    private static CreateAddressDto ValidCreateDto() =>
        new("12345-000", 100, AddressType.Clinic, "Sao Paulo", "Apt 1", "Brazil", "Center", "Near park", "SP");

    private static UpdateAddressDto ValidUpdateDto(Guid? id = null) =>
        new(id ?? Guid.NewGuid(), "12345-000", 100, AddressType.Clinic, "Sao Paulo", "Apt 1", "Brazil", "Center", "Near park", "SP");

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        _createValidator.Validate(ValidCreateDto()).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace postal code fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyPostalCode_Fails(string postalCode)
    {
        var dto = ValidCreateDto() with { PostalCode = postalCode };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PostalCode");
    }

    [Theory(DisplayName = "Create: postal code with invalid format fails validation")]
    [InlineData("a")]
    [InlineData("$$$$$")]
    [InlineData("123456789012345678901")]
    public void Create_InvalidPostalCodeFormat_Fails(string postalCode)
    {
        var dto = ValidCreateDto() with { PostalCode = postalCode };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PostalCode");
    }

    [Fact(DisplayName = "Create: empty city fails validation")]
    public void Create_EmptyCity_Fails()
    {
        var dto = ValidCreateDto() with { City = "" };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "City");
    }

    [Fact(DisplayName = "Create: city exceeding max length fails validation")]
    public void Create_CityTooLong_Fails()
    {
        var dto = ValidCreateDto() with { City = new string('a', 256) };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "City");
    }

    [Fact(DisplayName = "Create: empty country fails validation")]
    public void Create_EmptyCountry_Fails()
    {
        var dto = ValidCreateDto() with { Country = "" };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Country");
    }

    [Fact(DisplayName = "Create: country exceeding max length fails validation")]
    public void Create_CountryTooLong_Fails()
    {
        var dto = ValidCreateDto() with { Country = new string('a', 256) };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Country");
    }

    [Fact(DisplayName = "Create: empty state fails validation")]
    public void Create_EmptyState_Fails()
    {
        var dto = ValidCreateDto() with { State = "" };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "State");
    }

    [Fact(DisplayName = "Create: state exceeding max length fails validation")]
    public void Create_StateTooLong_Fails()
    {
        var dto = ValidCreateDto() with { State = new string('a', 101) };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "State");
    }

    [Fact(DisplayName = "Create: complement exceeding max length fails validation")]
    public void Create_ComplementTooLong_Fails()
    {
        var dto = ValidCreateDto() with { Complement = new string('a', 256) };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Complement");
    }

    [Fact(DisplayName = "Create: district exceeding max length fails validation")]
    public void Create_DistrictTooLong_Fails()
    {
        var dto = ValidCreateDto() with { District = new string('a', 256) };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "District");
    }

    [Fact(DisplayName = "Create: reference point exceeding max length fails validation")]
    public void Create_ReferencePointTooLong_Fails()
    {
        var dto = ValidCreateDto() with { ReferencePoint = new string('a', 256) };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ReferencePoint");
    }

    [Fact(DisplayName = "Create: invalid type fails validation")]
    public void Create_InvalidType_Fails()
    {
        var dto = ValidCreateDto() with { Type = (AddressType)999 };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Type");
    }

    [Fact(DisplayName = "Create: negative number fails validation")]
    public void Create_NegativeNumber_Fails()
    {
        var dto = ValidCreateDto() with { Number = -1 };
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Number");
    }

    [Fact(DisplayName = "Create: null optional fields pass validation")]
    public void Create_NullOptionalFields_Passes()
    {
        var dto = ValidCreateDto() with { Complement = null, District = null, ReferencePoint = null, Number = null };
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        _updateValidator.Validate(ValidUpdateDto()).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = ValidUpdateDto(Guid.Empty);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: postal code with invalid format fails validation")]
    [InlineData("")]
    [InlineData("$$$$$")]
    public void Update_InvalidPostalCodeFormat_Fails(string postalCode)
    {
        var dto = ValidUpdateDto() with { PostalCode = postalCode };
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "PostalCode");
    }

    [Fact(DisplayName = "Update: city exceeding max length fails validation")]
    public void Update_CityTooLong_Fails()
    {
        var dto = ValidUpdateDto() with { City = new string('a', 256) };
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "City");
    }

    [Fact(DisplayName = "Update: state exceeding max length fails validation")]
    public void Update_StateTooLong_Fails()
    {
        var dto = ValidUpdateDto() with { State = new string('a', 101) };
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "State");
    }

    [Fact(DisplayName = "Update: empty country fails validation")]
    public void Update_EmptyCountry_Fails()
    {
        var dto = ValidUpdateDto() with { Country = "" };
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Country");
    }
}
