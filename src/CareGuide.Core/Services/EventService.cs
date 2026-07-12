using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Event;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

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

        public async Task<PagedResult<EventDto>> GetAllByPersonAsync(
            DateTime? from,
            DateTime? to,
            EventSortOrder sortOrder,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            if (from.HasValue && to.HasValue && from.Value > to.Value)
                throw new ArgumentException("The 'from' date must not be later than the 'to' date.", nameof(from));

            var (normalizedPage, normalizedPageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var items = await _eventRepository.GetAllByPersonAsync(from, to, sortOrder, normalizedPage, normalizedPageSize, cancellationToken);
            var total = await _eventRepository.CountByPersonAsync(from, to, cancellationToken);

            return new PagedResult<EventDto>(_mapper.Map<List<EventDto>>(items), normalizedPage, normalizedPageSize, total);
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
