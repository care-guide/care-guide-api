using CareGuide.Core.Interfaces.Storage;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Configuration;
using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Security.Interfaces;
using Microsoft.Extensions.Options;
using EventEntity = CareGuide.Models.Entities.Event;

namespace CareGuide.Tests.Services;

public class EventFileUploadServiceTests
{
    private readonly IEventRepository _eventRepository = Substitute.For<IEventRepository>();
    private readonly IFileAssetRepository _fileRepository = Substitute.For<IFileAssetRepository>();
    private readonly IObjectStorage _storage = Substitute.For<IObjectStorage>();
    private readonly IUserSessionContext _session = Substitute.For<IUserSessionContext>();

    [Fact]
    public async Task InitiateAsync_WithOwnedEvent_ShouldCreatePendingFileAndReturnPutUrl()
    {
        var personId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        _session.PersonId.Returns(personId);
        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(CreateEvent(eventId, personId));
        _fileRepository.AddAsync(Arg.Any<FileAsset>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<FileAsset>());
        _storage.CreateUploadUrl(Arg.Any<string>(), "application/pdf", new string('a', 64), TimeSpan.FromMinutes(5))
            .Returns(new Uri("https://storage.test/upload"));
        var service = CreateService();
        var dto = new InitiateEventFileUploadDto("exam.pdf", 1024, "application/pdf", new string('A', 64));

        var result = await service.InitiateAsync(eventId, dto, CancellationToken.None);

        result.Method.Should().Be("PUT");
        result.UploadUrl.Should().Be(new Uri("https://storage.test/upload"));
        result.RequiredHeaders.Should().Contain("Content-Type", "application/pdf");
        result.RequiredHeaders.Should().Contain("x-amz-meta-sha256", new string('a', 64));
        result.ExpiresAt.Should().BeCloseTo(DateTimeOffset.UtcNow.AddMinutes(5), TimeSpan.FromSeconds(2));
        await _fileRepository.Received(1).AddAsync(
            Arg.Is<FileAsset>(file =>
                file.Id == result.FileId &&
                file.PersonId == personId &&
                file.UploadStatus == FileUploadStatus.Pending &&
                file.StorageKey.StartsWith($"person/{personId:N}/event/{eventId:N}/") &&
                file.Hash == new string('a', 64)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task InitiateAsync_WhenEventIsNotInPersonScope_ShouldThrowAndNotCreateFile()
    {
        _eventRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((EventEntity?)null);
        var service = CreateService();
        var dto = new InitiateEventFileUploadDto("exam.pdf", 1024, "application/pdf", new string('a', 64));

        var act = () => service.InitiateAsync(Guid.NewGuid(), dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _fileRepository.DidNotReceive().AddAsync(Arg.Any<FileAsset>(), Arg.Any<CancellationToken>());
        _storage.DidNotReceive().CreateUploadUrl(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<TimeSpan>());
    }

    private EventFileUploadService CreateService() => new(
        _eventRepository,
        _fileRepository,
        _storage,
        _session,
        Options.Create(new FileUploadOptions
        {
            MaxSizeBytes = 2048,
            AllowedContentTypes = ["application/pdf"],
            AllowedExtensions = [".pdf"],
            HashAlgorithm = "SHA256",
            HashFormat = "Hex",
            PresignedUrlExpirationSeconds = 300
        }));

    private static EventEntity CreateEvent(Guid id, Guid personId) => new()
    {
        Id = id,
        PersonId = personId,
        Name = "Exam",
        StartDate = DateTime.UtcNow,
        Type = EventType.Exam
    };
}
