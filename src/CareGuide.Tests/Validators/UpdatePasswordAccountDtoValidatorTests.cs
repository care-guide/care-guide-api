using CareGuide.Models.DTOs.Account;
using CareGuide.Models.Validators.Account;

namespace CareGuide.Tests.Validators;

public class UpdatePasswordAccountDtoValidatorTests
{
    private readonly UpdatePasswordAccountDtoValidator _validator = new();

    [Fact(DisplayName = "Valid passwords pass validation")]
    public void Validate_ValidPasswords_Passes()
    {
        var dto = new UpdatePasswordAccountDto("C0rrent@Pass!", "Str0ng@Pass!");
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Empty or whitespace current password fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyCurrentPassword_Fails(string currentPassword)
    {
        var dto = new UpdatePasswordAccountDto(currentPassword, "Str0ng@Pass!");
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CurrentPassword");
    }

    [Theory(DisplayName = "Empty or whitespace new password fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_EmptyNewPassword_Fails(string newPassword)
    {
        var dto = new UpdatePasswordAccountDto("C0rrent@Pass!", newPassword);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
    }

    [Fact(DisplayName = "Current password exceeding 255 chars fails validation")]
    public void Validate_CurrentPasswordTooLong_Fails()
    {
        var dto = new UpdatePasswordAccountDto(new string('a', 256), "Str0ng@Pass!");
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CurrentPassword");
    }

    [Fact(DisplayName = "New password exceeding 255 chars fails validation")]
    public void Validate_NewPasswordTooLong_Fails()
    {
        var dto = new UpdatePasswordAccountDto("C0rrent@Pass!", new string('a', 256));
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
    }

    [Fact(DisplayName = "Passwords of exactly 255 chars pass validation")]
    public void Validate_PasswordsMaxLength_Passes()
    {
        var dto = new UpdatePasswordAccountDto(new string('a', 255), new string('b', 255));
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }
}
