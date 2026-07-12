using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Core.Interfaces.Storage;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Core.Services
{
    public class EventFileService : IEventFileService
    {
        private readonly IEventFileRepository _eventFileRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IFileAssetRepository _fileAssetRepository;
        private readonly IMapper _mapper;
        private readonly IObjectStorage _objectStorage;
        private readonly IEfTransactionUnitOfWork _unitOfWork;

        public EventFileService(
            IEventFileRepository eventFileRepository,
            IEventRepository eventRepository,
            IFileAssetRepository fileAssetRepository,
            IMapper mapper,
            IObjectStorage objectStorage,
            IEfTransactionUnitOfWork unitOfWork)
        {
            _eventFileRepository = eventFileRepository;
            _eventRepository = eventRepository;
            _fileAssetRepository = fileAssetRepository;
            _mapper = mapper;
            _objectStorage = objectStorage;
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<EventFileDto>> GetAllByEventIdAsync(int page, int pageSize, Guid eventId, CancellationToken cancellationToken)
        {
            var (normalizedPage, normalizedPageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var eventFiles = await _eventFileRepository.GetAllByEventAsync(normalizedPage, normalizedPageSize, eventId, cancellationToken);
            var total = await _eventFileRepository.CountByEventAsync(eventId, cancellationToken);

            return new PagedResult<EventFileDto>(_mapper.Map<List<EventFileDto>>(eventFiles), normalizedPage, normalizedPageSize, total);
        }

        public async Task<EventFileDto> AttachAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken)
        {
            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);

            if (parentEvent == null)
                throw new UnauthorizedAccessException("You are not authorized to access this event or it does not exist.");

            var file = await _fileAssetRepository.GetAsync(fileId, cancellationToken);

            if (file == null)
                throw new KeyNotFoundException($"No file found with ID {fileId}.");
            if (file.UploadStatus != FileUploadStatus.Uploaded)
                throw new InvalidOperationException("Only completed uploads can be attached to an event.");

            var existing = await _eventFileRepository.GetByEventAndFileAsync(eventId, fileId, cancellationToken);

            if (existing != null)
                throw new InvalidOperationException($"The file with ID {fileId} is already attached to the event with ID {eventId}.");

            var entity = new EventFile { EventId = eventId, FileId = fileId };

            await _eventFileRepository.AddAsync(entity, cancellationToken);

            var created = await _eventFileRepository.GetByEventAndFileAsync(eventId, fileId, cancellationToken);

            if (created == null)
                throw new InvalidOperationException("The created file attachment could not be retrieved.");

            return _mapper.Map<EventFileDto>(created);
        }

        public async Task DetachAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken)
        {
            var eventFile = await _eventFileRepository.GetByEventAndFileAsync(eventId, fileId, cancellationToken);

            if (eventFile == null)
                throw new KeyNotFoundException($"No file attachment found for event {eventId} and file {fileId}.");

            await _eventFileRepository.DeleteAsync(eventFile, cancellationToken);
        }

        public async Task<EventFileDto> ConfirmUploadAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken)
        {
            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);
            if (parentEvent == null)
                throw new KeyNotFoundException($"No event found with ID {eventId}.");

            var file = await _fileAssetRepository.GetAsync(fileId, cancellationToken);
            if (file == null)
                throw new KeyNotFoundException($"No file found with ID {fileId}.");
            var existing = await _eventFileRepository.GetByEventAndFileAsync(eventId, fileId, cancellationToken);
            if (existing != null && file.UploadStatus == FileUploadStatus.Uploaded)
                return _mapper.Map<EventFileDto>(existing);
            if (existing != null || file.UploadStatus != FileUploadStatus.Pending)
                throw new InvalidOperationException("The file is not in a valid pending state for confirmation.");

            var metadata = await _objectStorage.GetMetadataAsync(file.StorageKey, cancellationToken);
            if (metadata == null)
                throw new KeyNotFoundException("The uploaded object was not found in private storage.");
            if (metadata.ContentLength != file.Size)
                throw new InvalidOperationException("The uploaded object size does not match the pending file.");
            if (!string.Equals(metadata.ContentType, file.ContentType, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The uploaded object content type does not match the pending file.");
            if (!string.Equals(metadata.ExpectedSha256, file.Hash, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("The uploaded object SHA-256 metadata does not match the pending file.");

            var attachment = new EventFile { EventId = eventId, FileId = fileId, File = file };
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _eventFileRepository.AddAsync(attachment, cancellationToken);
                file.UploadStatus = FileUploadStatus.Uploaded;
                await _fileAssetRepository.UpdateAsync(file, cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                file.UploadStatus = FileUploadStatus.Pending;
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

            return _mapper.Map<EventFileDto>(attachment);
        }
    }
}
