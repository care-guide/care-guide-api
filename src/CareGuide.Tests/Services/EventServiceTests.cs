using AutoMapper;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Event;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Tests.Services;

public class EventServiceTests
{
    private readonly IEventRepository _repository;
    private readonly IMapper _mapper;
    private readonly EventService _sut;

    private static readonly DateTime StartDate = new(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);

    public EventServiceTests()
    {
        _repository = Substitute.For<IEventRepository>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EventService(_repository, _mapper);
    }

    [Fact(DisplayName = "GetAsync: empty id throws ArgumentException")]
    public async Task GetAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.GetAsync(Guid.Empty, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*id*");
    }

    [Fact(DisplayName = "GetAsync: entity not found throws KeyNotFoundException")]
    public async Task GetAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.GetAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "GetAsync: found entity returns mapped DTO")]
    public async Task GetAsync_Found_ReturnsMappedDto()
    {
        var id = Guid.NewGuid();
        var entity = new Event { Id = id, PersonId = Guid.NewGuid(), Name = "Checkup", StartDate = StartDate, Type = EventType.Appointment };
        var expected = new EventDto(id, entity.PersonId!.Value, "Checkup", StartDate, null, EventType.Appointment, false, null, null, null, DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<EventDto>(entity).Returns(expected);

        var result = await _sut.GetAsync(id, CancellationToken.None);

        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CreateAsync: null dto throws ArgumentNullException")]
    public async Task CreateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.CreateAsync(null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "CreateAsync: valid dto maps, persists, and returns DTO")]
    public async Task CreateAsync_ValidDto_ReturnsMappedDto()
    {
        var createDto = new CreateEventDto("Checkup", StartDate, null, EventType.Appointment, false, null, null, null);
        var entity = new Event { PersonId = Guid.NewGuid(), Name = "Checkup", StartDate = StartDate, Type = EventType.Appointment };
        var expected = new EventDto(entity.Id, entity.PersonId!.Value, entity.Name, StartDate, null, EventType.Appointment, false, null, null, null, DateTime.UtcNow, DateTime.UtcNow);

        _mapper.Map<Event>(createDto).Returns(entity);
        _repository.AddAsync(entity, Arg.Any<CancellationToken>()).Returns(entity);
        _mapper.Map<EventDto>(entity).Returns(expected);

        var result = await _sut.CreateAsync(createDto, CancellationToken.None);

        result.Should().Be(expected);
        await _repository.Received(1).AddAsync(entity, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "UpdateAsync: empty id throws ArgumentException")]
    public async Task UpdateAsync_EmptyId_ThrowsArgumentException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.Empty, new UpdateEventDto(Guid.Empty, "Checkup", StartDate, null, EventType.Appointment, false, null, null, null), CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*id*");
    }

    [Fact(DisplayName = "UpdateAsync: null dto throws ArgumentNullException")]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        var act = async () => await _sut.UpdateAsync(Guid.NewGuid(), null!, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "UpdateAsync: entity not found throws KeyNotFoundException")]
    public async Task UpdateAsync_NotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns((Event?)null);

        var act = async () => await _sut.UpdateAsync(id, new UpdateEventDto(id, "New Name", StartDate, null, EventType.Appointment, false, null, null, null), CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{id}*");
    }

    [Fact(DisplayName = "UpdateAsync: valid data updates entity fields and returns DTO")]
    public async Task UpdateAsync_ValidData_UpdatesAndReturnsDto()
    {
        var id = Guid.NewGuid();
        var existing = new Event { Id = id, PersonId = Guid.NewGuid(), Name = "Old Name", StartDate = StartDate, Type = EventType.Appointment };
        var updateDto = new UpdateEventDto(id, "New Name", StartDate, null, EventType.Surgery, true, RecurrenceType.Surgery, RecurrenceFrequency.Weekly, "New Details");
        var updated = existing;
        var expected = new EventDto(id, existing.PersonId!.Value, "New Name", StartDate, null, EventType.Surgery, true, RecurrenceType.Surgery, RecurrenceFrequency.Weekly, "New Details", DateTime.UtcNow, DateTime.UtcNow);

        _repository.GetAsync(id, Arg.Any<CancellationToken>()).Returns(existing);
        _repository.UpdateAsync(existing, Arg.Any<CancellationToken>()).Returns(updated);
        _mapper.Map<EventDto>(updated).Returns(expected);

        var result = await _sut.UpdateAsync(id, updateDto, CancellationToken.None);

        result.Name.Should().Be("New Name");
        result.Type.Should().Be(EventType.Surgery);
        result.IsAllDay.Should().BeTrue();
        result.RecurrenceType.Should().Be(RecurrenceType.Surgery);
        result.RecurrenceFrequency.Should().Be(RecurrenceFrequency.Weekly);
        result.Details.Should().Be("New Details");
    }

    [Fact(DisplayName = "GetAllByPersonAsync: returns mapped list")]
    public async Task GetAllByPersonAsync_ReturnsPagedMappedList()
    {
        var events = new List<Event>
        {
            new() { PersonId = Guid.NewGuid(), Name = "Event A", StartDate = StartDate, Type = EventType.Appointment },
            new() { PersonId = Guid.NewGuid(), Name = "Event B", StartDate = StartDate, Type = EventType.Exam },
        };
        var dtos = events.Select(e => new EventDto(e.Id, e.PersonId!.Value, e.Name, e.StartDate, null, e.Type, false, null, null, null, DateTime.UtcNow, DateTime.UtcNow)).ToList();

        _repository.GetAllByPersonPagedAsync(1, 10, Arg.Any<CancellationToken>()).Returns((events, 2, 1, 10));
        _mapper.Map<List<EventDto>>(events).Returns(dtos);

        var result = await _sut.GetAllByPersonAsync(1, 10, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.Total.Should().Be(2);
        result.HasNext.Should().BeFalse();
    }

    [Fact(DisplayName = "DeleteAllByPersonAsync: delegates to repository")]
    public async Task DeleteAllByPersonAsync_DelegatesToRepository()
    {
        await _sut.DeleteAllByPersonAsync(CancellationToken.None);

        await _repository.Received(1).DeleteAllByPersonAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteByIdsAsync: delegates to repository")]
    public async Task DeleteByIdsAsync_DelegatesToRepository()
    {
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

        await _sut.DeleteByIdsAsync(ids, CancellationToken.None);

        await _repository.Received(1).DeleteManyAsync(ids, Arg.Any<CancellationToken>());
    }
}
