using AutoMapper;
using CareGuide.Models.DTOs.EventSurgery;

namespace CareGuide.Models.Mappers.EventSurgery
{
    public class EventSurgeryProfileMapper : Profile
    {
        public EventSurgeryProfileMapper()
        {
            CreateMap<Entities.EventSurgery, EventSurgeryDto>();
            CreateMap<CreateEventSurgeryDto, Entities.EventSurgery>();
            CreateMap<UpdateEventSurgeryDto, Entities.EventSurgery>();
        }
    }
}
