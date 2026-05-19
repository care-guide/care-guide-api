using CareGuide.Models.DTOs.Account;
using CareGuide.Models.Validators.Account;

namespace CareGuide.Tests.Validators;

public class UpdatePasswordAccountDtoValidatorTests
{
    private readonly UpdatePasswordAccountDtoValidator _validator = new();

    [Fact(DisplayName = "Valid password passes validation")]
    public void Validate_ValidPassword_Passes()
    {
        var dto = new UpdatePasswordAccountDto("Str0ng@Pass!");
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Empty or whitespace password fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyPassword_Fails(string password)
    {
        var dto = new UpdatePasswordAccountDto(password);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact(DisplayName = "Password exceeding 255 chars fails validation")]
    public void Validate_PasswordTooLong_Fails()
    {
        var dto = new UpdatePasswordAccountDto(new string('a', 256));
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact(DisplayName = "Password of exactly 255 chars passes validation")]
    public void Validate_PasswordMaxLength_Passes()
    {
        var dto = new UpdatePasswordAccountDto(new string('a', 255));
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }
}
