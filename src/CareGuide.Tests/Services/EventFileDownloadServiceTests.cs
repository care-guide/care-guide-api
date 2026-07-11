using CareGuide.Core.Interfaces.Storage;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Configuration;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.Extensions.Options;

namespace CareGuide.Tests.Services;

public class EventFileDownloadServiceTests
{
    private readonly IEventFileRepository _repository = Substitute.For<IEventFileRepository>();
    private readonly IObjectStorage _storage = Substitute.For<IObjectStorage>();

    [Fact]
    public async Task CreateDownloadAsync_WithAuthorizedUploadedFile_ShouldReturnShortLivedUrl()
    {
        var eventId = Guid.NewGuid();
        var file = CreateFile(FileUploadStatus.Uploaded);
        _repository.GetByEventAndFileAsync(eventId, file.Id, Arg.Any<CancellationToken>())
            .Returns(new EventFile { EventId = eventId, FileId = file.Id, File = file });
        _storage.GetMetadataAsync(file.StorageKey, Arg.Any<CancellationToken>())
            .Returns(new StorageObjectMetadata(file.StorageKey, file.Size, file.ContentType, "etag", file.Hash, DateTime.UtcNow));
        _storage.CreateDownloadUrl(file.StorageKey, file.Name, TimeSpan.FromMinutes(5))
            .Returns(new Uri("https://storage.test/download"));
        var service = CreateService();

        var result = await service.CreateDownloadAsync(eventId, file.Id, CancellationToken.None);

        result.FileId.Should().Be(file.Id);
        result.DownloadUrl.Should().Be(new Uri("https://storage.test/download"));
        result.ExpiresAt.Should().BeCloseTo(DateTimeOffset.UtcNow.AddMinutes(5), TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task CreateDownloadAsync_WhenAttachmentIsOutsidePersonScope_ShouldNotInspectStorage()
    {
        _repository.GetByEventAndFileAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((EventFile?)null);
        var service = CreateService();

        var act = () => service.CreateDownloadAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _storage.DidNotReceive().GetMetadataAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateDownloadAsync_WhenFileIsPending_ShouldNotInspectStorage()
    {
        var eventId = Guid.NewGuid();
        var file = CreateFile(FileUploadStatus.Pending);
        _repository.GetByEventAndFileAsync(eventId, file.Id, Arg.Any<CancellationToken>())
            .Returns(new EventFile { EventId = eventId, FileId = file.Id, File = file });
        var service = CreateService();

        var act = () => service.CreateDownloadAsync(eventId, file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _storage.DidNotReceive().GetMetadataAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateDownloadAsync_WhenObjectIsMissing_ShouldNotCreateUrl()
    {
        var eventId = Guid.NewGuid();
        var file = CreateFile(FileUploadStatus.Uploaded);
        _repository.GetByEventAndFileAsync(eventId, file.Id, Arg.Any<CancellationToken>())
            .Returns(new EventFile { EventId = eventId, FileId = file.Id, File = file });
        _storage.GetMetadataAsync(file.StorageKey, Arg.Any<CancellationToken>()).Returns((StorageObjectMetadata?)null);
        var service = CreateService();

        var act = () => service.CreateDownloadAsync(eventId, file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        _storage.DidNotReceive().CreateDownloadUrl(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>());
    }

    private EventFileDownloadService CreateService() => new(
        _repository,
        _storage,
        Options.Create(new FileDownloadOptions { PresignedUrlExpirationSeconds = 300 }));

    private static FileAsset CreateFile(FileUploadStatus status) => new()
    {
        Id = Guid.NewGuid(),
        PersonId = Guid.NewGuid(),
        Name = "exam.pdf",
        Size = 2048,
        Hash = new string('a', 64),
        StorageKey = $"person/event/{Guid.NewGuid():N}",
        ContentType = "application/pdf",
        UploadStatus = status
    };
}
