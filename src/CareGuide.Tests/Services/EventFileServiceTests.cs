using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class EventFileServiceTests
{
    private readonly IEventFileRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IMapper _mapper;
    private readonly EventFileService _sut;

    public EventFileServiceTests()
    {
        _repository = Substitute.For<IEventFileRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _fileAssetRepository = Substitute.For<IFileAssetRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EventFileService(_repository, _eventRepository, _fileAssetRepository, _mapper);
    }

    // ── GetAllByEventIdAsync ──────────────────────────────────────────────────

    [Fact(DisplayName = "GetAllByEventIdAsync: returns mapped list")]
    public async Task GetAllByEventIdAsync_ReturnsMappedList()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var eventFiles = new List<EventFile> { new() { EventId = eventId, FileId = fileId, File = new FileAsset { Id = fileId, Name = "a.pdf", Size = 1, Hash = "h" } } };
        var dtos = new List<EventFileDto> { new(eventId, fileId, "a.pdf", 1, "h") };

        _repository.GetAllByEventAsync(eventId, Arg.Any<CancellationToken>()).Returns(eventFiles);
        _mapper.Map<List<EventFileDto>>(eventFiles).Returns(dtos);

        var result = await _sut.GetAllByEventIdAsync(eventId, CancellationToken.None);

        result.Should().HaveCount(1);
    }

    [Fact(DisplayName = "GetAllByEventIdAsync: empty result returns empty collection")]
    public async Task GetAllByEventIdAsync_Empty_ReturnsEmptyCollection()
    {
        var eventId = Guid.NewGuid();
        _repository.GetAllByEventAsync(eventId, Arg.Any<CancellationToken>()).Returns(new List<EventFile>());
        _mapper.Map<List<EventFileDto>>(Arg.Any<List<EventFile>>()).Returns(new List<EventFileDto>());

        var result = await _sut.GetAllByEventIdAsync(eventId, CancellationToken.None);

        result.Should().BeEmpty();
    }

    // ── AttachAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "AttachAsync: event not owned by person throws UnauthorizedAccessException")]
    public async Task AttachAsync_EventNotOwned_ThrowsUnauthorizedAccessException()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.AttachAsync(eventId, fileId, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "AttachAsync: file not found throws KeyNotFoundException")]
    public async Task AttachAsync_FileNotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(fileId, Arg.Any<CancellationToken>()).Returns((FileAsset?)null);

        var act = async () => await _sut.AttachAsync(eventId, fileId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{fileId}*");
    }

    [Fact(DisplayName = "AttachAsync: already attached throws InvalidOperationException")]
    public async Task AttachAsync_AlreadyAttached_ThrowsInvalidOperationException()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(fileId, Arg.Any<CancellationToken>()).Returns(new FileAsset { Id = fileId, Name = "a.pdf", Size = 1, Hash = "h" });
        _repository.GetByEventAndFileAsync(eventId, fileId, Arg.Any<CancellationToken>()).Returns(new EventFile { EventId = eventId, FileId = fileId });

        var act = async () => await _sut.AttachAsync(eventId, fileId, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{fileId}*{eventId}*");
    }

    [Fact(DisplayName = "AttachAsync: valid attach persists entity and returns mapped DTO")]
    public async Task AttachAsync_Valid_PersistsAndReturnsDto()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var created = new EventFile { EventId = eventId, FileId = fileId, File = new FileAsset { Id = fileId, Name = "a.pdf", Size = 1, Hash = "h" } };
        var expected = new EventFileDto(eventId, fileId, "a.pdf", 1, "h");

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(fileId, Arg.Any<CancellationToken>()).Returns(new FileAsset { Id = fileId, Name = "a.pdf", Size = 1, Hash = "h" });
        _repository.GetByEventAndFileAsync(eventId, fileId, Arg.Any<CancellationToken>()).Returns((EventFile?)null, created);
        _mapper.Map<EventFileDto>(created).Returns(expected);

        var result = await _sut.AttachAsync(eventId, fileId, CancellationToken.None);

        await _repository.Received(1).AddAsync(Arg.Is<EventFile>(ef => ef.EventId == eventId && ef.FileId == fileId), Arg.Any<CancellationToken>());
        result.Should().Be(expected);
    }

    // ── DetachAsync ───────────────────────────────────────────────────────────

    [Fact(DisplayName = "DetachAsync: not found throws KeyNotFoundException")]
    public async Task DetachAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();

        _repository.GetByEventAndFileAsync(eventId, fileId, Arg.Any<CancellationToken>()).Returns((EventFile?)null);

        var act = async () => await _sut.DetachAsync(eventId, fileId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "DetachAsync: found calls repository DeleteAsync")]
    public async Task DetachAsync_Found_CallsDeleteAsync()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var eventFile = new EventFile { EventId = eventId, FileId = fileId };

        _repository.GetByEventAndFileAsync(eventId, fileId, Arg.Any<CancellationToken>()).Returns(eventFile);

        await _sut.DetachAsync(eventId, fileId, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(eventFile, Arg.Any<CancellationToken>());
    }
}
