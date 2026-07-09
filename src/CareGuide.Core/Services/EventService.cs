using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Event;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<List<EventDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var list = await _eventRepository.GetAllByPersonAsync(page, pageSize, cancellationToken);
            return _mapper.Map<List<EventDto>>(list);
        }

        public async Task<EventDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _eventRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No event found with the ID {id}.");

            return _mapper.Map<EventDto>(entity);
        }

        public async Task<EventDto> CreateAsync(CreateEventDto eventDto, CancellationToken cancellationToken)
        {
            if (eventDto == null)
                throw new ArgumentNullException(nameof(eventDto));

            var entity = _mapper.Map<Event>(eventDto);

            await _eventRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<EventDto>(entity);
        }

        public async Task<EventDto> UpdateAsync(Guid id, UpdateEventDto eventDto, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (eventDto == null)
                throw new ArgumentNullException(nameof(eventDto));

            var existing = await _eventRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No event found with the ID {id}.");

            existing.Name = eventDto.Name;
            existing.StartDate = eventDto.StartDate;
            existing.EndDate = eventDto.EndDate;
            existing.Type = eventDto.Type;
            existing.IsAllDay = eventDto.IsAllDay;
            existing.RecurrenceType = eventDto.RecurrenceType;
            existing.RecurrenceFrequency = eventDto.RecurrenceFrequency;
            existing.Details = eventDto.Details;

            var updated = await _eventRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<EventDto>(updated);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken)
        {
            await _eventRepository.DeleteAllByPersonAsync(cancellationToken);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _eventRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
