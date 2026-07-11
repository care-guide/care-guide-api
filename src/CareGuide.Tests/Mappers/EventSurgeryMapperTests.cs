using AutoMapper;
using CareGuide.Models.DTOs.EventSurgery;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
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
            Type = EventSurgeryType.Curative,
        };

        var dto = _mapper.Map<EventSurgeryDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Type.Should().Be(EventSurgeryType.Curative);
    }

    [Fact(DisplayName = "CreateEventSurgeryDto→EventSurgery: all fields map correctly")]
    public void Map_CreateEventSurgeryDtoToEntity_MapsFields()
    {
        var dto = new CreateEventSurgeryDto(EventSurgeryType.Diagnostic);

        var entity = _mapper.Map<EventSurgery>(dto);

        entity.Type.Should().Be(EventSurgeryType.Diagnostic);
    }

    [Fact(DisplayName = "UpdateEventSurgeryDto→EventSurgery: all fields map correctly")]
    public void Map_UpdateEventSurgeryDtoToEntity_MapsFields()
    {
        var dto = new UpdateEventSurgeryDto(EventSurgeryType.Reconstructive);

        var entity = _mapper.Map<EventSurgery>(dto);

        entity.Type.Should().Be(EventSurgeryType.Reconstructive);
    }
}
