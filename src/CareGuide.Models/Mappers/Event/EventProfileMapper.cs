using AutoMapper;
using CareGuide.Models.DTOs.Event;

namespace CareGuide.Models.Mappers.Event
{
    public class EventProfileMapper : Profile
    {
        public EventProfileMapper()
        {
            CreateMap<Entities.Event, EventDto>();
            CreateMap<CreateEventDto, Entities.Event>();
            CreateMap<UpdateEventDto, Entities.Event>();
        }
    }
}
