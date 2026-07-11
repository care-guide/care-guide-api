using AutoMapper;
using CareGuide.Core.Interfaces.Storage;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.Entities;

namespace CareGuide.Tests.Services;

public class EventFileServiceTests
{
    private readonly IEventFileRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IFileAssetRepository _fileAssetRepository;
    private readonly IMapper _mapper;
    private readonly IObjectStorage _objectStorage;
    private readonly IEfTransactionUnitOfWork _unitOfWork;
    private readonly EventFileService _sut;

    public EventFileServiceTests()
    {
        _repository = Substitute.For<IEventFileRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _fileAssetRepository = Substitute.For<IFileAssetRepository>();
        _mapper = Substitute.For<IMapper>();
        _objectStorage = Substitute.For<IObjectStorage>();
        _unitOfWork = Substitute.For<IEfTransactionUnitOfWork>();
        _sut = new EventFileService(_repository, _eventRepository, _fileAssetRepository, _mapper, _objectStorage, _unitOfWork);
    }

    // ── GetAllByEventIdAsync ──────────────────────────────────────────────────

    [Fact(DisplayName = "GetAllByEventIdAsync: returns mapped list")]
    public async Task GetAllByEventIdAsync_ReturnsMappedList()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var eventFiles = new List<EventFile> { new() { EventId = eventId, FileId = fileId, File = new FileAsset { Id = fileId, Name = "a.pdf", Size = 1, Hash = "h" } } };
        var dtos = new List<EventFileDto> { new(eventId, fileId, "a.pdf", 1, "h", "application/pdf") };

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
        _fileAssetRepository.GetAsync(fileId, Arg.Any<CancellationToken>()).Returns(new FileAsset { Id = fileId, Name = "a.pdf", Size = 1, Hash = "h", UploadStatus = Models.Enums.FileUploadStatus.Uploaded });
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
        var expected = new EventFileDto(eventId, fileId, "a.pdf", 1, "h", "application/pdf");

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(fileId, Arg.Any<CancellationToken>()).Returns(new FileAsset { Id = fileId, Name = "a.pdf", Size = 1, Hash = "h", UploadStatus = Models.Enums.FileUploadStatus.Uploaded });
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
        await _objectStorage.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmUploadAsync_WithMatchingObject_ShouldAttachAndCompleteAtomically()
    {
        var eventId = Guid.NewGuid();
        var fileId = Guid.NewGuid();
        var file = CreatePendingFile(fileId);
        var attachment = new EventFile { EventId = eventId, FileId = fileId, File = file };
        var expected = new EventFileDto(eventId, fileId, file.Name, file.Size, file.Hash, file.ContentType);
        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(fileId, Arg.Any<CancellationToken>()).Returns(file);
        _repository.GetByEventAndFileAsync(eventId, fileId, Arg.Any<CancellationToken>()).Returns((EventFile?)null);
        _objectStorage.GetMetadataAsync(file.StorageKey, Arg.Any<CancellationToken>()).Returns(
            new StorageObjectMetadata(file.StorageKey, file.Size, file.ContentType, "etag", file.Hash, DateTime.UtcNow));
        _mapper.Map<EventFileDto>(Arg.Any<EventFile>()).Returns(expected);

        var result = await _sut.ConfirmUploadAsync(eventId, fileId, CancellationToken.None);

        result.Should().Be(expected);
        file.UploadStatus.Should().Be(Models.Enums.FileUploadStatus.Uploaded);
        await _unitOfWork.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
        await _repository.Received(1).AddAsync(Arg.Is<EventFile>(value => value.EventId == eventId && value.FileId == fileId), Arg.Any<CancellationToken>());
        await _fileAssetRepository.Received(1).UpdateAsync(file, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmUploadAsync_WhenAlreadyCompleted_ShouldReturnExistingWithoutDuplicating()
    {
        var eventId = Guid.NewGuid();
        var file = CreatePendingFile(Guid.NewGuid());
        file.UploadStatus = Models.Enums.FileUploadStatus.Uploaded;
        var existing = new EventFile { EventId = eventId, FileId = file.Id, File = file };
        var expected = new EventFileDto(eventId, file.Id, file.Name, file.Size, file.Hash, file.ContentType);
        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        _repository.GetByEventAndFileAsync(eventId, file.Id, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<EventFileDto>(existing).Returns(expected);

        var result = await _sut.ConfirmUploadAsync(eventId, file.Id, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.DidNotReceive().AddAsync(Arg.Any<EventFile>(), Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmUploadAsync_WhenEventIsOutsidePersonScope_ShouldNotInspectStorage()
    {
        _eventRepository.GetAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = () => _sut.ConfirmUploadAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _objectStorage.DidNotReceive().GetMetadataAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmUploadAsync_WhenObjectDoesNotExist_ShouldNotStartTransaction()
    {
        var eventId = Guid.NewGuid();
        var file = CreatePendingFile(Guid.NewGuid());
        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        _objectStorage.GetMetadataAsync(file.StorageKey, Arg.Any<CancellationToken>()).Returns((StorageObjectMetadata?)null);

        var act = () => _sut.ConfirmUploadAsync(eventId, file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(2049, "application/pdf", true)]
    [InlineData(2048, "image/png", true)]
    [InlineData(2048, "application/pdf", false)]
    public async Task ConfirmUploadAsync_WithMismatchedMetadata_ShouldThrow(long size, string contentType, bool matchingHash)
    {
        var eventId = Guid.NewGuid();
        var file = CreatePendingFile(Guid.NewGuid());
        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        _objectStorage.GetMetadataAsync(file.StorageKey, Arg.Any<CancellationToken>()).Returns(
            new StorageObjectMetadata(file.StorageKey, size, contentType, "etag", matchingHash ? file.Hash : new string('b', 64), DateTime.UtcNow));

        var act = () => _sut.ConfirmUploadAsync(eventId, file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _unitOfWork.DidNotReceive().BeginTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmUploadAsync_WhenDatabaseWriteFails_ShouldRollbackAndKeepPendingStatus()
    {
        var eventId = Guid.NewGuid();
        var file = CreatePendingFile(Guid.NewGuid());
        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _fileAssetRepository.GetAsync(file.Id, Arg.Any<CancellationToken>()).Returns(file);
        _objectStorage.GetMetadataAsync(file.StorageKey, Arg.Any<CancellationToken>()).Returns(
            new StorageObjectMetadata(file.StorageKey, file.Size, file.ContentType, "etag", file.Hash, DateTime.UtcNow));
        _fileAssetRepository.UpdateAsync(file, Arg.Any<CancellationToken>()).ThrowsAsync(new InvalidOperationException("write failed"));

        var act = () => _sut.ConfirmUploadAsync(eventId, file.Id, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        file.UploadStatus.Should().Be(Models.Enums.FileUploadStatus.Pending);
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    private static FileAsset CreatePendingFile(Guid id) => new()
    {
        Id = id,
        PersonId = Guid.NewGuid(),
        Name = "exam.pdf",
        Size = 2048,
        Hash = new string('a', 64),
        StorageKey = $"person/event/{id:N}",
        ContentType = "application/pdf",
        UploadStatus = Models.Enums.FileUploadStatus.Pending
    };
}
