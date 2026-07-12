using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.Enums;
using CareGuide.Models.Validators.Person;

namespace CareGuide.Tests.Validators;

public class UpdatePersonProfileDtoValidatorTests
{
    private readonly UpdatePersonProfileDtoValidator _validator = new();

    [Fact(DisplayName = "Valid dto passes validation")]
    public void ValidDto_Passes()
    {
        var dto = new UpdatePersonProfileDto("Alice", Gender.F, new DateOnly(1990, 1, 1), "http://pic.url");
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Null picture passes validation")]
    public void NullPicture_Passes()
    {
        var dto = new UpdatePersonProfileDto("Alice", Gender.F, new DateOnly(1990, 1, 1), null);
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Empty name fails validation")]
    public void EmptyName_Fails()
    {
        var dto = new UpdatePersonProfileDto("", Gender.F, new DateOnly(1990, 1, 1), null);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Name exceeding max length fails validation")]
    public void NameTooLong_Fails()
    {
        var dto = new UpdatePersonProfileDto(new string('a', DatabaseConstants.MaxLengthStandardText + 1), Gender.F, new DateOnly(1990, 1, 1), null);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Invalid gender fails validation")]
    public void InvalidGender_Fails()
    {
        var dto = new UpdatePersonProfileDto("Alice", (Gender)999, new DateOnly(1990, 1, 1), null);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Gender");
    }

    [Fact(DisplayName = "Future birthday fails validation")]
    public void FutureBirthday_Fails()
    {
        var dto = new UpdatePersonProfileDto("Alice", Gender.F, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), null);
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Birthday");
    }

    [Fact(DisplayName = "Today as birthday passes validation")]
    public void TodayBirthday_Passes()
    {
        var dto = new UpdatePersonProfileDto("Alice", Gender.F, DateOnly.FromDateTime(DateTime.UtcNow), null);
        _validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Picture exceeding max length fails validation")]
    public void PictureTooLong_Fails()
    {
        var dto = new UpdatePersonProfileDto("Alice", Gender.F, new DateOnly(1990, 1, 1), new string('a', DatabaseConstants.MaxLengthStandardText + 1));
        var result = _validator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Picture");
    }
}
