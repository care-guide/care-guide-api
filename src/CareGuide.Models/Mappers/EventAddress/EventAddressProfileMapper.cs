using AutoMapper;
using CareGuide.Models.DTOs.EventAddress;

namespace CareGuide.Models.Mappers.EventAddress
{
    public class EventAddressProfileMapper : Profile
    {
        public EventAddressProfileMapper()
        {
            CreateMap<Entities.EventAddress, EventAddressDto>()
                .ForCtorParam(nameof(EventAddressDto.EventId), opt => opt.MapFrom(src => src.EventId))
                .ForCtorParam(nameof(EventAddressDto.AddressId), opt => opt.MapFrom(src => GetAddress(src).Id))
                .ForCtorParam(nameof(EventAddressDto.PostalCode), opt => opt.MapFrom(src => GetAddress(src).PostalCode))
                .ForCtorParam(nameof(EventAddressDto.Number), opt => opt.MapFrom(src => GetAddress(src).Number))
                .ForCtorParam(nameof(EventAddressDto.Type), opt => opt.MapFrom(src => GetAddress(src).Type))
                .ForCtorParam(nameof(EventAddressDto.City), opt => opt.MapFrom(src => GetAddress(src).City))
                .ForCtorParam(nameof(EventAddressDto.Complement), opt => opt.MapFrom(src => GetAddress(src).Complement))
                .ForCtorParam(nameof(EventAddressDto.Country), opt => opt.MapFrom(src => GetAddress(src).Country))
                .ForCtorParam(nameof(EventAddressDto.District), opt => opt.MapFrom(src => GetAddress(src).District))
                .ForCtorParam(nameof(EventAddressDto.ReferencePoint), opt => opt.MapFrom(src => GetAddress(src).ReferencePoint))
                .ForCtorParam(nameof(EventAddressDto.State), opt => opt.MapFrom(src => GetAddress(src).State));
        }

        private static Entities.Address GetAddress(Entities.EventAddress source)
        {
            return source.Address
                ?? throw new InvalidOperationException("Address not found.");
        }
    }
}
