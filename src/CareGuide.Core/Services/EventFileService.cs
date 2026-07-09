using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class EventFileService : IEventFileService
    {
        private readonly IEventFileRepository _eventFileRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IFileAssetRepository _fileAssetRepository;
        private readonly IMapper _mapper;

        public EventFileService(IEventFileRepository eventFileRepository, IEventRepository eventRepository, IFileAssetRepository fileAssetRepository, IMapper mapper)
        {
            _eventFileRepository = eventFileRepository;
            _eventRepository = eventRepository;
            _fileAssetRepository = fileAssetRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<EventFileDto>> GetAllByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var eventFiles = await _eventFileRepository.GetAllByEventAsync(eventId, cancellationToken);

            return _mapper.Map<List<EventFileDto>>(eventFiles);
        }

        public async Task<EventFileDto> AttachAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken)
        {
            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);

            if (parentEvent == null)
                throw new UnauthorizedAccessException("You are not authorized to access this event or it does not exist.");

            var file = await _fileAssetRepository.GetAsync(fileId, cancellationToken);

            if (file == null)
                throw new KeyNotFoundException($"No file found with ID {fileId}.");

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
    }
}
