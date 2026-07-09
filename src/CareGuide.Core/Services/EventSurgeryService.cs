using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventSurgery;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class EventSurgeryService : IEventSurgeryService
    {
        private readonly IEventSurgeryRepository _eventSurgeryRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventSurgeryService(IEventSurgeryRepository eventSurgeryRepository, IEventRepository eventRepository, IMapper mapper)
        {
            _eventSurgeryRepository = eventSurgeryRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventSurgeryDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var surgery = await _eventSurgeryRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (surgery == null)
                throw new KeyNotFoundException($"No surgery found for the event with ID {eventId}.");

            return _mapper.Map<EventSurgeryDto>(surgery);
        }

        public async Task<EventSurgeryDto> CreateAsync(Guid eventId, CreateEventSurgeryDto eventSurgeryDto, CancellationToken cancellationToken)
        {
            if (eventSurgeryDto == null)
                throw new ArgumentNullException(nameof(eventSurgeryDto));

            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);

            if (parentEvent == null)
                throw new UnauthorizedAccessException("You are not authorized to access this event or it does not exist.");

            var existing = await _eventSurgeryRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (existing != null)
                throw new InvalidOperationException($"A surgery already exists for the event with ID {eventId}.");

            var entity = _mapper.Map<EventSurgery>(eventSurgeryDto);
            entity.EventId = eventId;

            var created = await _eventSurgeryRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<EventSurgeryDto>(created);
        }

        public async Task<EventSurgeryDto> UpdateAsync(Guid eventId, UpdateEventSurgeryDto eventSurgeryDto, CancellationToken cancellationToken)
        {
            if (eventSurgeryDto == null)
                throw new ArgumentNullException(nameof(eventSurgeryDto));

            var surgery = await _eventSurgeryRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (surgery == null)
                throw new KeyNotFoundException($"No surgery found for the event with ID {eventId}.");

            surgery.Type = eventSurgeryDto.Type;

            var updated = await _eventSurgeryRepository.UpdateAsync(surgery, cancellationToken);

            return _mapper.Map<EventSurgeryDto>(updated);
        }

        public async Task DeleteAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var surgery = await _eventSurgeryRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (surgery == null)
                throw new KeyNotFoundException($"No surgery found for the event with ID {eventId}.");

            await _eventSurgeryRepository.DeleteAsync(surgery.Id, cancellationToken);
        }
    }
}
