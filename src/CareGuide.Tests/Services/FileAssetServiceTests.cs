using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class FileAssetServiceTests
{
    private readonly IFileAssetRepository _repository;
    private readonly IMapper _mapper;
    private readonly FileAssetService _sut;

    public FileAssetServiceTests()
    {
        _repository = Substitute.For<IFileAssetRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new FileAssetService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAllAsync: reads only files scoped to the session person")]
    public async Task GetAllAsync_UsesPersonOwnedQuery()
    {
        _repository.GetAllByPersonAsync(1, 10, Arg.Any<CancellationToken>()).Returns([]);

        await _sut.GetAllAsync(1, 10, CancellationToken.None);

        await _repository.Received(1).GetAllByPersonAsync(1, 10, Arg.Any<CancellationToken>());
        await _repository.DidNotReceive().GetAllAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GetAsync: empty id throws ArgumentException")]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.GetAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "GetAsync: file not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((FileAsset?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "GetAsync: found returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var entity = new FileAsset { Id = id, Name = "report.pdf", Size = 2048, Hash = new string('a', 64) };
        var expected = new FileAssetDto(id, "report.pdf", 2048, new string('a', 64), "application/pdf", StorageProvider.CloudflareR2, FileUploadStatus.Uploaded, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<FileAssetDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: valid dto persists and returns DTO")]
    public async Task CreateAsync_ValidDto_PersistsAndReturnsDto()
    {
        var createDto = new CreateFileAssetDto("report.pdf", 2048, new string('a', 64), "application/pdf");
        var entity = new FileAsset { Name = "report.pdf", Size = 2048, Hash = new string('a', 64) };
        var expected = new FileAssetDto(entity.Id, "report.pdf", 2048, new string('a', 64), "application/pdf", StorageProvider.CloudflareR2, FileUploadStatus.Pending, DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<FileAsset>(createDto).Returns(entity);
        _mapper.Map<FileAssetDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        entity.StorageKey.Should().MatchRegex("^files/[a-f0-9]{32}$");
        entity.StorageKey.Should().NotContain(createDto.Name);
        entity.StorageProvider.Should().Be(StorageProvider.CloudflareR2);
        entity.UploadStatus.Should().Be(FileUploadStatus.Pending);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdateFileAssetDto(Guid.Empty, "report.pdf", 2048, new string('a', 64), "application/pdf"), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*id*");
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: file not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((FileAsset?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdateFileAssetDto(id, "report.pdf", 2048, new string('a', 64), "application/pdf"), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var existing = new FileAsset { Id = id, Name = "old.pdf", Size = 1024, Hash = new string('b', 64) };
        var updateDto = new UpdateFileAssetDto(id, "new.pdf", 4096, new string('c', 64), "application/pdf");
        var expected = new FileAssetDto(id, "new.pdf", 4096, new string('c', 64), "application/pdf", StorageProvider.CloudflareR2, FileUploadStatus.Uploaded, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<FileAssetDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        existing.Name.Should().Be("new.pdf");
        existing.Size.Should().Be(4096);
        existing.Hash.Should().Be(new string('c', 64));
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteByIdsAsync: delegates to repository DeleteManyAsync")]
    public async Task DeleteByIdsAsync_Delegates_ToRepositoryDeleteManyAsync()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        await _sut.DeleteByIdsAsync(ids, CancellationToken.None);

        await _repository.Received(1).DeleteManyAsync(ids, Arg.Any<CancellationToken>());
    }
}
