using AutoMapper;
using CareGuide.Models.DTOs.Event;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.Event;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class EventMapperTests
{
    private readonly IMapper _mapper;

    public EventMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    // ── Event → EventDto ──────────────────────────────────────────────────────

    [Fact(DisplayName = "Event→EventDto: all fields map correctly")]
    public void Map_EventToDto_MapsAllFields()
    {
        var entity = new Event
        {
            Id = Guid.NewGuid(),
            PersonId = Guid.NewGuid(),
            Name = "Cardiology appointment",
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 1, 1, 11, 0, 0),
            Type = EventType.Appointment,
            IsAllDay = false,
            RecurrenceType = RecurrenceType.Weekly,
            RecurrenceFrequency = 2,
            Details = "Bring exam results",
            CreatedAt = new DateTime(2024, 1, 1),
            UpdatedAt = new DateTime(2024, 6, 1),
        };

        var dto = _mapper.Map<EventDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.PersonId.Should().Be(entity.PersonId!.Value);
        dto.Name.Should().Be(entity.Name);
        dto.StartDate.Should().Be(entity.StartDate);
        dto.EndDate.Should().Be(entity.EndDate);
        dto.Type.Should().Be(entity.Type);
        dto.IsAllDay.Should().Be(entity.IsAllDay);
        dto.RecurrenceType.Should().Be(entity.RecurrenceType);
        dto.RecurrenceFrequency.Should().Be(entity.RecurrenceFrequency);
        dto.Details.Should().Be(entity.Details);
        dto.CreatedAt.Should().Be(entity.CreatedAt);
        dto.UpdatedAt.Should().Be(entity.UpdatedAt);
    }

    [Fact(DisplayName = "Event→EventDto: null recurrence and details map to null")]
    public void Map_EventToDto_NullRecurrenceAndDetails_MapsToNull()
    {
        var entity = new Event
        {
            PersonId = Guid.NewGuid(),
            Name = "Exam",
            StartDate = new DateTime(2026, 1, 1),
            Type = EventType.Exam,
            RecurrenceType = null,
            RecurrenceFrequency = null,
            Details = null,
        };

        var dto = _mapper.Map<EventDto>(entity);

        dto.RecurrenceType.Should().BeNull();
        dto.RecurrenceFrequency.Should().BeNull();
        dto.Details.Should().BeNull();
    }

    // ── CreateEventDto → Event ────────────────────────────────────────────────

    [Fact(DisplayName = "CreateEventDto→Event: all fields map correctly")]
    public void Map_CreateDtoToEvent_MapsAllFields()
    {
        var dto = new CreateEventDto("Surgery", new DateTime(2026, 2, 1), new DateTime(2026, 2, 1, 14, 0, 0), EventType.Surgery, false, RecurrenceType.Monthly, 1, "Pre-op notes");

        var entity = _mapper.Map<Event>(dto);

        entity.Name.Should().Be("Surgery");
        entity.StartDate.Should().Be(dto.StartDate);
        entity.EndDate.Should().Be(dto.EndDate);
        entity.Type.Should().Be(EventType.Surgery);
        entity.RecurrenceType.Should().Be(RecurrenceType.Monthly);
        entity.RecurrenceFrequency.Should().Be(1);
        entity.Details.Should().Be("Pre-op notes");
    }

    // ── UpdateEventDto → Event ────────────────────────────────────────────────

    [Fact(DisplayName = "UpdateEventDto→Event: all fields map correctly")]
    public void Map_UpdateDtoToEvent_MapsAllFields()
    {
        var dto = new UpdateEventDto(Guid.NewGuid(), "Medicine reminder", new DateTime(2026, 3, 1), null, EventType.Medicine, true, null, null, null);

        var entity = _mapper.Map<Event>(dto);

        entity.Name.Should().Be("Medicine reminder");
        entity.StartDate.Should().Be(dto.StartDate);
        entity.Type.Should().Be(EventType.Medicine);
        entity.IsAllDay.Should().BeTrue();
    }
}
