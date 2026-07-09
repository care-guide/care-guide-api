using AutoMapper;
using CareGuide.Models.DTOs.EventAppointment;

namespace CareGuide.Models.Mappers.EventAppointment
{
    public class EventAppointmentProfileMapper : Profile
    {
        public EventAppointmentProfileMapper()
        {
            CreateMap<Entities.EventAppointment, EventAppointmentDto>();
            CreateMap<CreateEventAppointmentDto, Entities.EventAppointment>();
            CreateMap<UpdateEventAppointmentDto, Entities.EventAppointment>();
        }
    }
}
