using AutoMapper;
using CareGuide.Models.DTOs.EventFile;

namespace CareGuide.Models.Mappers.EventFile
{
    public class EventFileProfileMapper : Profile
    {
        public EventFileProfileMapper()
        {
            CreateMap<Entities.EventFile, EventFileDto>()
                .ForCtorParam(nameof(EventFileDto.EventId), opt => opt.MapFrom(src => src.EventId))
                .ForCtorParam(nameof(EventFileDto.FileId), opt => opt.MapFrom(src => GetFile(src).Id))
                .ForCtorParam(nameof(EventFileDto.Name), opt => opt.MapFrom(src => GetFile(src).Name))
                .ForCtorParam(nameof(EventFileDto.Size), opt => opt.MapFrom(src => GetFile(src).Size))
                .ForCtorParam(nameof(EventFileDto.Hash), opt => opt.MapFrom(src => GetFile(src).Hash));
        }

        private static Entities.FileAsset GetFile(Entities.EventFile source)
        {
            return source.File
                ?? throw new InvalidOperationException("File not found.");
        }
    }
}
