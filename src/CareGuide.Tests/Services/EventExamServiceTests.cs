using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventExam;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class EventExamServiceTests
{
    private readonly IEventExamRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly EventExamService _sut;

    public EventExamServiceTests()
    {
        _repository = Substitute.For<IEventExamRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EventExamService(_repository, _eventRepository, _mapper);
    }

    [Fact(DisplayName = "GetByEventIdAsync: not found throws KeyNotFoundException")]
    public async Task GetByEventIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventExam?)null);

        var act = async () => await _sut.GetByEventIdAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "GetByEventIdAsync: found returns mapped DTO")]
    public async Task GetByEventIdAsync_Found_ReturnsMappedDto()
    {
        var eventId = Guid.NewGuid();
        var exam = new EventExam { EventId = eventId, Type = EventExamType.Blood };
        var expected = new EventExamDto(exam.Id, eventId, EventExamType.Blood);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(exam);
        _mapper.Map<EventExamDto>(exam).Returns(expected);

        var result = await _sut.GetByEventIdAsync(eventId, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: event not owned by person throws UnauthorizedAccessException")]
    public async Task CreateAsync_EventNotOwned_ThrowsUnauthorizedAccessException()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventExamDto(EventExamType.Blood);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "CreateAsync: exam record already exists throws InvalidOperationException")]
    public async Task CreateAsync_AlreadyExists_ThrowsInvalidOperationException()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventExamDto(EventExamType.Blood);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(new EventExam { EventId = eventId, Type = EventExamType.Blood });

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "CreateAsync: valid dto sets EventId and persists entity")]
    public async Task CreateAsync_ValidDto_SetsEventIdAndPersists()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventExamDto(EventExamType.Blood);
        var entity = new EventExam { EventId = Guid.Empty, Type = EventExamType.Blood };
        var expected = new EventExamDto(entity.Id, eventId, EventExamType.Blood);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Exam });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventExam?)null);
        _mapper.Map<EventExam>(dto).Returns(entity);
        _repository.AddAsync(Arg.Any<EventExam>(), Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<EventExamDto>(entity).Returns(expected);

        await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        entity.EventId.Should().Be(eventId);
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        var dto = new UpdateEventExamDto(EventExamType.Blood);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventExam?)null);

        var act = async () => await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var eventId = Guid.NewGuid();
        var existing = new EventExam { EventId = eventId, Type = EventExamType.Blood };
        var dto = new UpdateEventExamDto(EventExamType.Imaging);
        var expected = new EventExamDto(existing.Id, eventId, EventExamType.Imaging);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<EventExamDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        existing.Type.Should().Be(EventExamType.Imaging);
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteAsync: not found throws KeyNotFoundException")]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventExam?)null);

        var act = async () => await _sut.DeleteAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "DeleteAsync: found calls repository DeleteAsync with exam id")]
    public async Task DeleteAsync_Found_CallsDeleteAsync()
    {
        var eventId = Guid.NewGuid();
        var exam = new EventExam { EventId = eventId, Type = EventExamType.Blood };

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(exam);

        await _sut.DeleteAsync(eventId, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(exam.Id, Arg.Any<CancellationToken>());
    }
}
