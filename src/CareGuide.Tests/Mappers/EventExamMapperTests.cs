using AutoMapper;
using CareGuide.Models.DTOs.EventExam;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.EventExam;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class EventExamMapperTests
{
    private readonly IMapper _mapper;

    public EventExamMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventExamProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "EventExam→EventExamDto: all fields map correctly")]
    public void Map_EventExamToDto_MapsAllFields()
    {
        var eventId = Guid.NewGuid();
        var entity = new EventExam
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Type = EventExamType.Blood,
        };

        var dto = _mapper.Map<EventExamDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Type.Should().Be(EventExamType.Blood);
    }

    [Fact(DisplayName = "CreateEventExamDto→EventExam: all fields map correctly")]
    public void Map_CreateEventExamDtoToEntity_MapsFields()
    {
        var dto = new CreateEventExamDto(EventExamType.Imaging);

        var entity = _mapper.Map<EventExam>(dto);

        entity.Type.Should().Be(EventExamType.Imaging);
    }

    [Fact(DisplayName = "UpdateEventExamDto→EventExam: all fields map correctly")]
    public void Map_UpdateEventExamDtoToEntity_MapsFields()
    {
        var dto = new UpdateEventExamDto(EventExamType.Genetic);

        var entity = _mapper.Map<EventExam>(dto);

        entity.Type.Should().Be(EventExamType.Genetic);
    }
}
