using AutoMapper;
using CareGuide.Models.DTOs.EventAppointment;
using CareGuide.Models.Entities;
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
            Type = "Checkup",
        };

        var dto = _mapper.Map<EventAppointmentDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Doctor.Should().Be("Dr. House");
        dto.Type.Should().Be("Checkup");
    }

    [Fact(DisplayName = "EventAppointment→EventAppointmentDto: null fields map to null")]
    public void Map_EventAppointmentToDto_NullFields_MapToNull()
    {
        var entity = new EventAppointment { EventId = Guid.NewGuid(), Doctor = null, Type = null };

        var dto = _mapper.Map<EventAppointmentDto>(entity);

        dto.Doctor.Should().BeNull();
        dto.Type.Should().BeNull();
    }

    [Fact(DisplayName = "CreateEventAppointmentDto→EventAppointment: all fields map correctly")]
    public void Map_CreateEventAppointmentDtoToEntity_MapsFields()
    {
        var dto = new CreateEventAppointmentDto("Dr. House", "Checkup");

        var entity = _mapper.Map<EventAppointment>(dto);

        entity.Doctor.Should().Be("Dr. House");
        entity.Type.Should().Be("Checkup");
    }

    [Fact(DisplayName = "UpdateEventAppointmentDto→EventAppointment: all fields map correctly")]
    public void Map_UpdateEventAppointmentDtoToEntity_MapsFields()
    {
        var dto = new UpdateEventAppointmentDto("New Doctor", "New Type");

        var entity = _mapper.Map<EventAppointment>(dto);

        entity.Doctor.Should().Be("New Doctor");
        entity.Type.Should().Be("New Type");
    }
}
