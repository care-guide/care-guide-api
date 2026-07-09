using AutoMapper;
using CareGuide.Models.DTOs.FileAsset;

namespace CareGuide.Models.Mappers.FileAsset
{
    public class FileAssetProfileMapper : Profile
    {
        public FileAssetProfileMapper()
        {
            CreateMap<Entities.FileAsset, FileAssetDto>();
            CreateMap<CreateFileAssetDto, Entities.FileAsset>();
            CreateMap<UpdateFileAssetDto, Entities.FileAsset>();
        }
    }
}
