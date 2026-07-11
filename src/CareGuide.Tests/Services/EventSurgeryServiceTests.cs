using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventSurgery;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class EventSurgeryServiceTests
{
    private readonly IEventSurgeryRepository _repository;
    private readonly IEventRepository _eventRepository;
    private readonly IMapper _mapper;
    private readonly EventSurgeryService _sut;

    public EventSurgeryServiceTests()
    {
        _repository = Substitute.For<IEventSurgeryRepository>();
        _eventRepository = Substitute.For<IEventRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EventSurgeryService(_repository, _eventRepository, _mapper);
    }

    [Fact(DisplayName = "GetByEventIdAsync: not found throws KeyNotFoundException")]
    public async Task GetByEventIdAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventSurgery?)null);

        var act = async () => await _sut.GetByEventIdAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "GetByEventIdAsync: found returns mapped DTO")]
    public async Task GetByEventIdAsync_Found_ReturnsMappedDto()
    {
        var eventId = Guid.NewGuid();
        var surgery = new EventSurgery { EventId = eventId, Type = EventSurgeryType.Curative };
        var expected = new EventSurgeryDto(surgery.Id, eventId, EventSurgeryType.Curative);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(surgery);
        _mapper.Map<EventSurgeryDto>(surgery).Returns(expected);

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
        var dto = new CreateEventSurgeryDto(EventSurgeryType.Curative);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "CreateAsync: surgery already exists throws InvalidOperationException")]
    public async Task CreateAsync_AlreadyExists_ThrowsInvalidOperationException()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventSurgeryDto(EventSurgeryType.Curative);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Surgery });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(new EventSurgery { EventId = eventId, Type = EventSurgeryType.Curative });

        var act = async () => await _sut.CreateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "CreateAsync: valid dto sets EventId and persists entity")]
    public async Task CreateAsync_ValidDto_SetsEventIdAndPersists()
    {
        var eventId = Guid.NewGuid();
        var dto = new CreateEventSurgeryDto(EventSurgeryType.Curative);
        var entity = new EventSurgery { EventId = Guid.Empty, Type = EventSurgeryType.Curative };
        var expected = new EventSurgeryDto(entity.Id, eventId, EventSurgeryType.Curative);

        _eventRepository.GetAsync(eventId, Arg.Any<CancellationToken>()).Returns(new Event { Id = eventId, PersonId = Guid.NewGuid(), Name = "Event", StartDate = DateTime.UtcNow, Type = Models.Enums.EventType.Surgery });
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventSurgery?)null);
        _mapper.Map<EventSurgery>(dto).Returns(entity);
        _repository.AddAsync(Arg.Any<EventSurgery>(), Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<EventSurgeryDto>(entity).Returns(expected);

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
        var dto = new UpdateEventSurgeryDto(EventSurgeryType.Curative);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventSurgery?)null);

        var act = async () => await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesFieldsAndReturnsDto()
    {
        var eventId = Guid.NewGuid();
        var existing = new EventSurgery { EventId = eventId, Type = EventSurgeryType.Curative };
        var dto = new UpdateEventSurgeryDto(EventSurgeryType.Palliative);
        var expected = new EventSurgeryDto(existing.Id, eventId, EventSurgeryType.Palliative);

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(existing);
        _mapper.Map<EventSurgeryDto>(existing).Returns(expected);

        var result = await _sut.UpdateAsync(eventId, dto, CancellationToken.None);

        existing.Type.Should().Be(EventSurgeryType.Palliative);
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "DeleteAsync: not found throws KeyNotFoundException")]
    public async Task DeleteAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var eventId = Guid.NewGuid();
        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns((EventSurgery?)null);

        var act = async () => await _sut.DeleteAsync(eventId, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"*{eventId}*");
    }

    [Fact(DisplayName = "DeleteAsync: found calls repository DeleteAsync with surgery id")]
    public async Task DeleteAsync_Found_CallsDeleteAsync()
    {
        var eventId = Guid.NewGuid();
        var surgery = new EventSurgery { EventId = eventId, Type = EventSurgeryType.Curative };

        _repository.GetByEventIdAsync(eventId, Arg.Any<CancellationToken>()).Returns(surgery);

        await _sut.DeleteAsync(eventId, CancellationToken.None);

        await _repository.Received(1).DeleteAsync(surgery.Id, Arg.Any<CancellationToken>());
    }
}
