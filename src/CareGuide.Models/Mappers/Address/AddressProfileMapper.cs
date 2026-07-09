using AutoMapper;
using CareGuide.Models.DTOs.Address;

namespace CareGuide.Models.Mappers.Address
{
    public class AddressProfileMapper : Profile
    {
        public AddressProfileMapper()
        {
            CreateMap<Entities.Address, AddressDto>();
            CreateMap<CreateAddressDto, Entities.Address>();
            CreateMap<UpdateAddressDto, Entities.Address>();
        }
    }
}
