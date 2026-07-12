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
        var doctorId = Guid.NewGuid();
        var entity = new EventAppointment
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            Doctor = "Dr. House",
            DoctorId = doctorId,
            DoctorRef = new Doctor { Id = doctorId, PersonId = Guid.NewGuid(), Name = "Dr. House" },
            Type = EventAppointmentType.Routine,
        };

        var dto = _mapper.Map<EventAppointmentDto>(entity);

        dto.Id.Should().Be(entity.Id);
        dto.EventId.Should().Be(eventId);
        dto.Doctor.Should().Be("Dr. House");
        dto.DoctorId.Should().Be(doctorId);
        dto.DoctorName.Should().Be("Dr. House");
        dto.Type.Should().Be(EventAppointmentType.Routine);
    }

    [Fact(DisplayName = "EventAppointment→EventAppointmentDto: null Doctor maps to null")]
    public void Map_EventAppointmentToDto_NullDoctor_MapsToNull()
    {
        var entity = new EventAppointment { EventId = Guid.NewGuid(), Doctor = null, Type = EventAppointmentType.Emergency };

        var dto = _mapper.Map<EventAppointmentDto>(entity);

        dto.Doctor.Should().BeNull();
        dto.DoctorId.Should().BeNull();
        dto.DoctorName.Should().BeNull();
        dto.Type.Should().Be(EventAppointmentType.Emergency);
    }

    [Fact(DisplayName = "CreateEventAppointmentDto→EventAppointment: all fields map correctly")]
    public void Map_CreateEventAppointmentDtoToEntity_MapsFields()
    {
        var doctorId = Guid.NewGuid();
        var dto = new CreateEventAppointmentDto("Dr. House", doctorId, EventAppointmentType.FollowUp);

        var entity = _mapper.Map<EventAppointment>(dto);

        entity.Doctor.Should().Be("Dr. House");
        entity.DoctorId.Should().Be(doctorId);
        entity.Type.Should().Be(EventAppointmentType.FollowUp);
    }

    [Fact(DisplayName = "UpdateEventAppointmentDto→EventAppointment: all fields map correctly")]
    public void Map_UpdateEventAppointmentDtoToEntity_MapsFields()
    {
        var doctorId = Guid.NewGuid();
        var dto = new UpdateEventAppointmentDto("New Doctor", doctorId, EventAppointmentType.Specialized);

        var entity = _mapper.Map<EventAppointment>(dto);

        entity.Doctor.Should().Be("New Doctor");
        entity.DoctorId.Should().Be(doctorId);
        entity.Type.Should().Be(EventAppointmentType.Specialized);
    }
}
