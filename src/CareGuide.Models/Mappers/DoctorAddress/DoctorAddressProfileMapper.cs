using AutoMapper;
using CareGuide.Models.DTOs.DoctorAddress;

namespace CareGuide.Models.Mappers.DoctorAddress
{
    public class DoctorAddressProfileMapper : Profile
    {
        public DoctorAddressProfileMapper()
        {
            CreateMap<Entities.DoctorAddress, DoctorAddressDto>()
                .ForCtorParam(nameof(DoctorAddressDto.DoctorId), opt => opt.MapFrom(src => src.DoctorId))
                .ForCtorParam(nameof(DoctorAddressDto.AddressId), opt => opt.MapFrom(src => GetAddress(src).Id))
                .ForCtorParam(nameof(DoctorAddressDto.PostalCode), opt => opt.MapFrom(src => GetAddress(src).PostalCode))
                .ForCtorParam(nameof(DoctorAddressDto.Number), opt => opt.MapFrom(src => GetAddress(src).Number))
                .ForCtorParam(nameof(DoctorAddressDto.Type), opt => opt.MapFrom(src => GetAddress(src).Type))
                .ForCtorParam(nameof(DoctorAddressDto.City), opt => opt.MapFrom(src => GetAddress(src).City))
                .ForCtorParam(nameof(DoctorAddressDto.Complement), opt => opt.MapFrom(src => GetAddress(src).Complement))
                .ForCtorParam(nameof(DoctorAddressDto.Country), opt => opt.MapFrom(src => GetAddress(src).Country))
                .ForCtorParam(nameof(DoctorAddressDto.District), opt => opt.MapFrom(src => GetAddress(src).District))
                .ForCtorParam(nameof(DoctorAddressDto.ReferencePoint), opt => opt.MapFrom(src => GetAddress(src).ReferencePoint))
                .ForCtorParam(nameof(DoctorAddressDto.State), opt => opt.MapFrom(src => GetAddress(src).State));
        }

        private static Entities.Address GetAddress(Entities.DoctorAddress source)
        {
            return source.Address
                ?? throw new InvalidOperationException("Address not found.");
        }
    }
}
