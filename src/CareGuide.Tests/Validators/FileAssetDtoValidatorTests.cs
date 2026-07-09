using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Validators.FileAsset;

namespace CareGuide.Tests.Validators;

public class FileAssetDtoValidatorTests
{
    private readonly CreateFileAssetDtoValidator _createValidator = new();
    private readonly UpdateFileAssetDtoValidator _updateValidator = new();

    private static readonly string ValidHash = new('a', 64);

    [Fact(DisplayName = "Create: valid dto passes validation")]
    public void Create_ValidDto_Passes()
    {
        var dto = new CreateFileAssetDto("report.pdf", 2048, ValidHash);
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Create: empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_Fails(string name)
    {
        var dto = new CreateFileAssetDto(name, 2048, ValidHash);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact(DisplayName = "Create: name longer than max length fails validation")]
    public void Create_NameTooLong_Fails()
    {
        var dto = new CreateFileAssetDto(new string('x', 256), 2048, ValidHash);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory(DisplayName = "Create: size less than or equal to zero fails validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_SizeNotPositive_Fails(long size)
    {
        var dto = new CreateFileAssetDto("report.pdf", size, ValidHash);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Size");
    }

    [Theory(DisplayName = "Create: empty or whitespace hash fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyHash_Fails(string hash)
    {
        var dto = new CreateFileAssetDto("report.pdf", 2048, hash);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Hash");
    }

    [Theory(DisplayName = "Create: malformed hash fails validation")]
    [InlineData("not-a-hash")]
    [InlineData("12345")]
    [InlineData("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz")]
    public void Create_MalformedHash_Fails(string hash)
    {
        var dto = new CreateFileAssetDto("report.pdf", 2048, hash);
        var result = _createValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Hash");
    }

    [Fact(DisplayName = "Create: valid sha256 hex hash passes validation")]
    public void Create_ValidSha256Hash_Passes()
    {
        var dto = new CreateFileAssetDto("report.pdf", 2048, new string('f', 64));
        _createValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: valid dto passes validation")]
    public void Update_ValidDto_Passes()
    {
        var dto = new UpdateFileAssetDto(Guid.NewGuid(), "report.pdf", 2048, ValidHash);
        _updateValidator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Update: empty Id fails validation")]
    public void Update_EmptyId_Fails()
    {
        var dto = new UpdateFileAssetDto(Guid.Empty, "report.pdf", 2048, ValidHash);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Theory(DisplayName = "Update: empty or whitespace name fails validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_EmptyName_Fails(string name)
    {
        var dto = new UpdateFileAssetDto(Guid.NewGuid(), name, 2048, ValidHash);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory(DisplayName = "Update: size less than or equal to zero fails validation")]
    [InlineData(0)]
    [InlineData(-100)]
    public void Update_SizeNotPositive_Fails(long size)
    {
        var dto = new UpdateFileAssetDto(Guid.NewGuid(), "report.pdf", size, ValidHash);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Size");
    }

    [Theory(DisplayName = "Update: malformed hash fails validation")]
    [InlineData("not-a-hash")]
    [InlineData("12345")]
    public void Update_MalformedHash_Fails(string hash)
    {
        var dto = new UpdateFileAssetDto(Guid.NewGuid(), "report.pdf", 2048, hash);
        var result = _updateValidator.Validate(dto);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Hash");
    }
}
