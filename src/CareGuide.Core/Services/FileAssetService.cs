using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Core.Services
{
    public class FileAssetService : IFileAssetService
    {
        private readonly IFileAssetRepository _fileAssetRepository;
        private readonly IMapper _mapper;

        public FileAssetService(IFileAssetRepository fileAssetRepository, IMapper mapper)
        {
            _fileAssetRepository = fileAssetRepository;
            _mapper = mapper;
        }

        public async Task<List<FileAssetDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var list = await _fileAssetRepository.GetAllByPersonAsync(page, pageSize, cancellationToken);
            return _mapper.Map<List<FileAssetDto>>(list);
        }

        public async Task<FileAssetDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _fileAssetRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No file found with the ID {id}.");

            return _mapper.Map<FileAssetDto>(entity);
        }

        public async Task<FileAssetDto> CreateAsync(CreateFileAssetDto createFileAssetDto, CancellationToken cancellationToken)
        {
            if (createFileAssetDto == null)
                throw new ArgumentNullException(nameof(createFileAssetDto));

            var entity = _mapper.Map<FileAsset>(createFileAssetDto);
            entity.StorageKey = $"files/{Guid.NewGuid():N}";
            entity.StorageProvider = StorageProvider.CloudflareR2;
            entity.UploadStatus = FileUploadStatus.Pending;

            await _fileAssetRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<FileAssetDto>(entity);
        }

        public async Task<FileAssetDto> UpdateAsync(Guid id, UpdateFileAssetDto updateFileAssetDto, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (updateFileAssetDto == null)
                throw new ArgumentNullException(nameof(updateFileAssetDto));

            var existing = await _fileAssetRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No file found with the ID {id}.");

            existing.Name = updateFileAssetDto.Name;
            existing.Size = updateFileAssetDto.Size;
            existing.Hash = updateFileAssetDto.Hash;
            existing.ContentType = updateFileAssetDto.ContentType;

            var updated = await _fileAssetRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<FileAssetDto>(updated);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _fileAssetRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
