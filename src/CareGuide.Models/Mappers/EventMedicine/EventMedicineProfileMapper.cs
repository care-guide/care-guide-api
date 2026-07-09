using AutoMapper;
using CareGuide.Models.DTOs.EventMedicine;

namespace CareGuide.Models.Mappers.EventMedicine
{
    public class EventMedicineProfileMapper : Profile
    {
        public EventMedicineProfileMapper()
        {
            CreateMap<Entities.EventMedicine, EventMedicineDto>();
            CreateMap<CreateEventMedicineDto, Entities.EventMedicine>();
            CreateMap<UpdateEventMedicineDto, Entities.EventMedicine>();
        }
    }
}
