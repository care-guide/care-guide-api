using AutoMapper;
using CareGuide.Models.DTOs.FileAsset;

namespace CareGuide.Models.Mappers.FileAsset
{
    public class FileAssetProfileMapper : Profile
    {
        public FileAssetProfileMapper()
        {
            // StorageKey stays on the entity only; FileAssetDto has no member for it, so it is
            // never serialized to clients (see docs/SECURITY.md §7).
            CreateMap<Entities.FileAsset, FileAssetDto>();
            CreateMap<CreateFileAssetDto, Entities.FileAsset>();
            CreateMap<UpdateFileAssetDto, Entities.FileAsset>();
        }
    }
}
