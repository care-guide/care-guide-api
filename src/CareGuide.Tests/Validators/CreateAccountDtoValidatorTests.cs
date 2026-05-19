using CareGuide.Models.DTOs.Account;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.Account;

namespace CareGuide.Tests.Validators;

public class CreateAccountDtoValidatorTests
{
    private readonly CreateAccountDtoValidator _validator = new();

    private static CreateAccountDto Valid() => new(
        "user@test.com",
        "Str0ng@Pass!",
        "Test User",
        Gender.F,
        DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
    );

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
        var dto = Valid() with { Email = new string('a', 250) + "@x.com" };

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

    [Theory(DisplayName = "Empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyName_FailsWithRequiredMessage(string name)
    {
        var dto = Valid() with { Name = name };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required.");
    }

    [Fact(DisplayName = "Future birthday fails validation")]
    public void FutureBirthday_FailsValidation()
    {
        var dto = Valid() with { Birthday = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Birthday");
    }

    [Fact(DisplayName = "Today birthday passes validation")]
    public void TodayBirthday_PassesValidation()
    {
        var dto = Valid() with { Birthday = DateOnly.FromDateTime(DateTime.Now) };

        var result = _validator.Validate(dto);

        result.Errors.Should().NotContain(e => e.PropertyName == "Birthday");
    }

    [Fact(DisplayName = "Invalid gender value fails validation")]
    public void InvalidGender_FailsValidation()
    {
        var dto = Valid() with { Gender = (Gender)99 };

        var result = _validator.Validate(dto);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Gender");
    }
}
