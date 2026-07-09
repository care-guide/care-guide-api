using AutoMapper;
using CareGuide.Models.DTOs.EventSurgery;
using CareGuide.Models.Entities;
using CareGuide.Models.Mappers.EventSurgery;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class EventSurgeryMapperTests
{
    private readonly IMapper _mapper;

    public EventSurgeryMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventSurgeryProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "EventSurgery→EventSurgeryDto: all fields map correctly")]
    public void Map_EventSurgeryToDto_MapsAllFields()
    {
        var eventId = Guid.NewGuid();
        var entity = new EventSurgery
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Type = "Appendectomy",
        };

        var dto = _mapper.Map<EventSurgeryDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Type.Should().Be("Appendectomy");
    }

    [Fact(DisplayName = "EventSurgery→EventSurgeryDto: null Type maps to null")]
    public void Map_EventSurgeryToDto_NullType_MapsToNull()
    {
        var entity = new EventSurgery { EventId = Guid.NewGuid(), Type = null };

        var dto = _mapper.Map<EventSurgeryDto>(entity);

        dto.Type.Should().BeNull();
    }

    [Fact(DisplayName = "CreateEventSurgeryDto→EventSurgery: all fields map correctly")]
    public void Map_CreateEventSurgeryDtoToEntity_MapsFields()
    {
        var dto = new CreateEventSurgeryDto("Appendectomy");

        var entity = _mapper.Map<EventSurgery>(dto);

        entity.Type.Should().Be("Appendectomy");
    }

    [Fact(DisplayName = "UpdateEventSurgeryDto→EventSurgery: all fields map correctly")]
    public void Map_UpdateEventSurgeryDtoToEntity_MapsFields()
    {
        var dto = new UpdateEventSurgeryDto("New Type");

        var entity = _mapper.Map<EventSurgery>(dto);

        entity.Type.Should().Be("New Type");
    }
}
