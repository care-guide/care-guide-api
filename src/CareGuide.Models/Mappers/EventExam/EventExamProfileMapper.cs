using AutoMapper;
using CareGuide.Models.DTOs.EventExam;

namespace CareGuide.Models.Mappers.EventExam
{
    public class EventExamProfileMapper : Profile
    {
        public EventExamProfileMapper()
        {
            CreateMap<Entities.EventExam, EventExamDto>();
            CreateMap<CreateEventExamDto, Entities.EventExam>();
            CreateMap<UpdateEventExamDto, Entities.EventExam>();
        }
    }
}
