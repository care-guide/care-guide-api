using CareGuide.Core.Interfaces.Storage;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class FileLifecycleServiceTests
{
    private readonly IFileAssetRepository _files = Substitute.For<IFileAssetRepository>();
    private readonly IEventFileRepository _eventFiles = Substitute.For<IEventFileRepository>();
    private readonly IObjectStorage _storage = Substitute.For<IObjectStorage>();

    [Fact]
    public async Task DeleteAsync_WhenUnreferenced_ShouldMarkDeleteStorageAndRemoveRecord()
    {
        var file = CreateFile();
        _files.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        var service = new FileLifecycleService(_files, _eventFiles, _storage);

        await service.DeleteAsync(file.Id, CancellationToken.None);

        await _files.Received(1).UpdateAsync(Arg.Is<FileAsset>(value => value.UploadStatus == FileUploadStatus.DeletionPending), Arg.Any<CancellationToken>());
        await _storage.Received(1).DeleteAsync(file.StorageKey, Arg.Any<CancellationToken>());
        await _files.Received(1).DeleteAsync(file.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenReferenced_ShouldPreserveStorageAndDatabase()
    {
        var file = CreateFile();
        _files.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        _eventFiles.CountByFileAsync(file.Id, Arg.Any<CancellationToken>()).Returns(2);
        var service = new FileLifecycleService(_files, _eventFiles, _storage);

        var act = () => service.DeleteAsync(file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _storage.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _files.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenStorageFails_ShouldRemainObservableAndRetryable()
    {
        var file = CreateFile();
        _files.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        _storage.DeleteAsync(file.StorageKey, Arg.Any<CancellationToken>()).ThrowsAsync(new HttpRequestException("storage unavailable"));
        var service = new FileLifecycleService(_files, _eventFiles, _storage);

        var act = () => service.DeleteAsync(file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<HttpRequestException>();
        file.UploadStatus.Should().Be(FileUploadStatus.DeletionFailed);
        await _files.Received(2).UpdateAsync(file, Arg.Any<CancellationToken>());
        await _files.DidNotReceive().DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenDatabaseDeleteFails_ShouldRemainDeletionPendingForRetry()
    {
        var file = CreateFile();
        _files.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        _files.DeleteAsync(file.Id, Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("database unavailable"));
        var service = new FileLifecycleService(_files, _eventFiles, _storage);

        var act = () => service.DeleteAsync(file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        file.UploadStatus.Should().Be(FileUploadStatus.DeletionPending);
        await _storage.Received(1).DeleteAsync(file.StorageKey, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WhenFileIsOutsideOwnerScope_ShouldNotTouchStorage()
    {
        _files.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((FileAsset?)null);
        var service = new FileLifecycleService(_files, _eventFiles, _storage);

        var act = () => service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _storage.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    private static FileAsset CreateFile() => new()
    {
        Id = Guid.NewGuid(),
        PersonId = Guid.NewGuid(),
        Name = "exam.pdf",
        Size = 1,
        Hash = new string('a', 64),
        StorageKey = $"files/{Guid.NewGuid():N}",
        ContentType = "application/pdf",
        UploadStatus = FileUploadStatus.Uploaded
    };
}
