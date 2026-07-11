using AutoMapper;
using CareGuide.Models.DTOs.EventAppointment;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Models.Mappers.EventAppointment;
using Microsoft.Extensions.DependencyInjection;

namespace CareGuide.Tests.Mappers;

public class EventAppointmentMapperTests
{
    private readonly IMapper _mapper;

    public EventAppointmentMapperTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<EventAppointmentProfileMapper>();
        });
        _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();
    }

    [Fact(DisplayName = "EventAppointment→EventAppointmentDto: all fields map correctly")]
    public void Map_EventAppointmentToDto_MapsAllFields()
    {
        var eventId = Guid.NewGuid();
        var entity = new EventAppointment
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Doctor = "Dr. House",
            Type = EventAppointmentType.Routine,
        };

        var dto = _mapper.Map<EventAppointmentDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Doctor.Should().Be("Dr. House");
        dto.Type.Should().Be(EventAppointmentType.Routine);
    }

    [Fact(DisplayName = "EventAppointment→EventAppointmentDto: null Doctor maps to null")]
    public void Map_EventAppointmentToDto_NullDoctor_MapsToNull()
    {
        var entity = new EventAppointment { EventId = Guid.NewGuid(), Doctor = null, Type = EventAppointmentType.Emergency };

        var dto = _mapper.Map<EventAppointmentDto>(entity);

        dto.Doctor.Should().BeNull();
        dto.Type.Should().Be(EventAppointmentType.Emergency);
    }

    [Fact(DisplayName = "CreateEventAppointmentDto→EventAppointment: all fields map correctly")]
    public void Map_CreateEventAppointmentDtoToEntity_MapsFields()
    {
        var dto = new CreateEventAppointmentDto("Dr. House", EventAppointmentType.FollowUp);

        var entity = _mapper.Map<EventAppointment>(dto);

        entity.Doctor.Should().Be("Dr. House");
        entity.Type.Should().Be(EventAppointmentType.FollowUp);
    }

    [Fact(DisplayName = "UpdateEventAppointmentDto→EventAppointment: all fields map correctly")]
    public void Map_UpdateEventAppointmentDtoToEntity_MapsFields()
    {
        var dto = new UpdateEventAppointmentDto("New Doctor", EventAppointmentType.Specialized);

        var entity = _mapper.Map<EventAppointment>(dto);

        entity.Doctor.Should().Be("New Doctor");
        entity.Type.Should().Be(EventAppointmentType.Specialized);
    }
}
