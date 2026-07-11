using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.Validators.Account;

namespace CareGuide.Tests.Validators;

public class LoginAccountDtoValidatorTests
{
    private readonly LoginAccountDtoValidator _validator = new();

    private static LoginAccountDto Valid() => new("user@test.com", "Str0ng@Pass!");

    [Fact(DisplayName = "Valid DTO passes validation")]
    public void ValidDto_PassesValidation()
    {
        var result = _validator.Validate(Valid());

        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Empty or whitespace email fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyEmail_FailsWithRequiredMessage(string email)
    {
        var dto = Valid() with { Email = email };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Email is required.");
    }

    [Fact(DisplayName = "Invalid email format fails validation")]
    public void InvalidEmailFormat_FailsValidation()
    {
        var dto = Valid() with { Email = "not-an-email" };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact(DisplayName = "Email exceeding max length fails validation")]
    public void EmailTooLong_FailsValidation()
    {
        var dto = Valid() with { Email = new string('a', DatabaseConstants.MaxLengthStandardText) + "@x.com" };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Theory(DisplayName = "Empty or whitespace password fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyPassword_FailsWithRequiredMessage(string password)
    {
        var dto = Valid() with { Password = password };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password" && e.ErrorMessage == "Password is required.");
    }

    [Fact(DisplayName = "Password exceeding max length fails validation")]
    public void PasswordTooLong_FailsValidation()
    {
        var dto = Valid() with { Password = new string('a', DatabaseConstants.MaxLengthStandardText + 1) };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact(DisplayName = "Password with max length passes validation")]
    public void PasswordMaxLength_PassesValidation()
    {
        var dto = Valid() with { Password = new string('a', DatabaseConstants.MaxLengthStandardText) };

        var result = _validator.Validate(dto);

        result.Errors.Should().NotContain(e => e.PropertyName == "Password");
    }
}
