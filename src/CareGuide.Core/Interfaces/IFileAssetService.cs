using CareGuide.Models.DTOs.FileAsset;

namespace CareGuide.Core.Interfaces
{
    public interface IFileAssetService
    {
        Task<List<FileAssetDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<FileAssetDto> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<FileAssetDto> CreateAsync(CreateFileAssetDto createFileAssetDto, CancellationToken cancellationToken);
        Task<FileAssetDto> UpdateAsync(Guid id, UpdateFileAssetDto updateFileAssetDto, CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}
