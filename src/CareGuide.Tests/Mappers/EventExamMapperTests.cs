using AutoMapper;
using CareGuide.Models.DTOs.EventExam;
using CareGuide.Models.Entities;
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
            Type = "Blood Test",
        };

        var dto = _mapper.Map<EventExamDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Type.Should().Be("Blood Test");
    }

    [Fact(DisplayName = "EventExam→EventExamDto: null fields map to null")]
    public void Map_EventExamToDto_NullFields_MapToNull()
    {
        var entity = new EventExam { EventId = Guid.NewGuid(), Type = null };

        var dto = _mapper.Map<EventExamDto>(entity);

        dto.Type.Should().BeNull();
    }

    [Fact(DisplayName = "CreateEventExamDto→EventExam: all fields map correctly")]
    public void Map_CreateEventExamDtoToEntity_MapsFields()
    {
        var dto = new CreateEventExamDto("Blood Test");

        var entity = _mapper.Map<EventExam>(dto);

        entity.Type.Should().Be("Blood Test");
    }

    [Fact(DisplayName = "UpdateEventExamDto→EventExam: all fields map correctly")]
    public void Map_UpdateEventExamDtoToEntity_MapsFields()
    {
        var dto = new UpdateEventExamDto("New Type");

        var entity = _mapper.Map<EventExam>(dto);

        entity.Type.Should().Be("New Type");
    }
}
