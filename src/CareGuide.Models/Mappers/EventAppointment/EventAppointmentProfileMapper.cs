using AutoMapper;
using CareGuide.Models.DTOs.EventAppointment;

namespace CareGuide.Models.Mappers.EventAppointment
{
    public class EventAppointmentProfileMapper : Profile
    {
        public EventAppointmentProfileMapper()
        {
            CreateMap<Entities.EventAppointment, EventAppointmentDto>()
                .ForCtorParam(nameof(EventAppointmentDto.DoctorName), opt => opt.MapFrom(src => src.DoctorRef != null ? src.DoctorRef.Name : null));
            CreateMap<CreateEventAppointmentDto, Entities.EventAppointment>();
            CreateMap<UpdateEventAppointmentDto, Entities.EventAppointment>();
        }
    }
}
