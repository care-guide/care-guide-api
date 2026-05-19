using CareGuide.Security.Helpers;

namespace CareGuide.Tests.Helpers;

public class PasswordManagerHelperTests
{
    [Fact(DisplayName = "HashPassword: returns non-empty hash different from plain text")]
    public void HashPassword_ValidInput_ReturnsDifferentHash()
    {
        var plain = "Str0ng@Pass!";

        var hash = PasswordManagerHelper.HashPassword(plain);

        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(plain);
    }

    [Fact(DisplayName = "HashPassword: same input produces different hashes (salt-based)")]
    public void HashPassword_SameInput_ProducesDifferentHashes()
    {
        var plain = "Str0ng@Pass!";

        var hash1 = PasswordManagerHelper.HashPassword(plain);
        var hash2 = PasswordManagerHelper.HashPassword(plain);

        hash1.Should().NotBe(hash2, because: "BCrypt uses a random salt per hash");
    }

    [Fact(DisplayName = "ValidatePassword: correct plain text matches its hash")]
    public void ValidatePassword_CorrectPassword_ReturnsTrue()
    {
        var plain = "Str0ng@Pass!";
        var hash = PasswordManagerHelper.HashPassword(plain);

        var result = PasswordManagerHelper.ValidatePassword(plain, hash);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "ValidatePassword: wrong password does not match hash")]
    public void ValidatePassword_WrongPassword_ReturnsFalse()
    {
        var hash = PasswordManagerHelper.HashPassword("Str0ng@Pass!");

        var result = PasswordManagerHelper.ValidatePassword("Wr0ngP@ss!", hash);

        result.Should().BeFalse();
    }

    [Theory(DisplayName = "CheckPassword: empty or whitespace password returns false with feedback")]
    [InlineData("")]
    [InlineData("   ")]
    public void CheckPassword_EmptyPassword_ReturnsFalseWithFeedback(string password)
    {
        var (isSecure, feedback) = PasswordManagerHelper.CheckPassword(password);

        isSecure.Should().BeFalse();
        feedback.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "CheckPassword: password missing uppercase returns false")]
    public void CheckPassword_MissingUppercase_ReturnsFalse()
    {
        var (isSecure, feedback) = PasswordManagerHelper.CheckPassword("str0ng@pass!");

        isSecure.Should().BeFalse();
        feedback.Should().Contain("uppercase");
    }

    [Fact(DisplayName = "CheckPassword: password missing lowercase returns false")]
    public void CheckPassword_MissingLowercase_ReturnsFalse()
    {
        var (isSecure, feedback) = PasswordManagerHelper.CheckPassword("STR0NG@PASS!");

        isSecure.Should().BeFalse();
        feedback.Should().Contain("lowercase");
    }

    [Fact(DisplayName = "CheckPassword: password missing digit returns false")]
    public void CheckPassword_MissingDigit_ReturnsFalse()
    {
        var (isSecure, feedback) = PasswordManagerHelper.CheckPassword("Strong@Pass!");

        isSecure.Should().BeFalse();
        feedback.Should().Contain("number");
    }

    [Fact(DisplayName = "CheckPassword: password missing special character returns false")]
    public void CheckPassword_MissingSpecialChar_ReturnsFalse()
    {
        var (isSecure, feedback) = PasswordManagerHelper.CheckPassword("Str0ngPass1");

        isSecure.Should().BeFalse();
        feedback.Should().Contain("special character");
    }

    [Fact(DisplayName = "CheckPassword: strong password returns true with empty feedback")]
    public void CheckPassword_StrongPassword_ReturnsTrueWithEmptyFeedback()
    {
        var (isSecure, feedback) = PasswordManagerHelper.CheckPassword("Str0ng@Pass!2024");

        isSecure.Should().BeTrue();
        feedback.Should().BeEmpty();
    }
}
