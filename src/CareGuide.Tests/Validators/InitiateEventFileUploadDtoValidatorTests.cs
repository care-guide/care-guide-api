using CareGuide.Models.Configuration;
using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Validators.FileAsset;
using Microsoft.Extensions.Options;

namespace CareGuide.Tests.Validators;

public class InitiateEventFileUploadDtoValidatorTests
{
    private readonly InitiateEventFileUploadDtoValidator _validator = new(Options.Create(CreateOptions()));

    [Fact]
    public void Validate_WithValidFile_ShouldSucceed()
    {
        var result = _validator.Validate(new InitiateEventFileUploadDto(
            "exam.pdf", 1024, "application/pdf", new string('a', 64)));

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("exam.exe", 1024, "application/pdf")]
    [InlineData("exam.pdf", 2049, "application/pdf")]
    [InlineData("exam.pdf", 1024, "application/octet-stream")]
    public void Validate_WithDisallowedFile_ShouldFail(string name, long size, string contentType)
    {
        var result = _validator.Validate(new InitiateEventFileUploadDto(
            name, size, contentType, new string('a', 64)));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WithInvalidHash_ShouldFail()
    {
        var result = _validator.Validate(new InitiateEventFileUploadDto(
            "exam.pdf", 1024, "application/pdf", "not-a-sha256-hash"));

        result.IsValid.Should().BeFalse();
    }

    private static FileUploadOptions CreateOptions() => new()
    {
        MaxSizeBytes = 2048,
        AllowedContentTypes = ["application/pdf"],
        AllowedExtensions = [".pdf"],
        HashAlgorithm = "SHA256",
        HashFormat = "Hex",
        PresignedUrlExpirationSeconds = 300
    };
}
