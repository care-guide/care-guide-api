using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventExam;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class EventExamService : IEventExamService
    {
        private readonly IEventExamRepository _eventExamRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventExamService(IEventExamRepository eventExamRepository, IEventRepository eventRepository, IMapper mapper)
        {
            _eventExamRepository = eventExamRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventExamDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var exam = await _eventExamRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException($"No exam record found for the event with ID {eventId}.");

            return _mapper.Map<EventExamDto>(exam);
        }

        public async Task<EventExamDto> CreateAsync(Guid eventId, CreateEventExamDto eventExamDto, CancellationToken cancellationToken)
        {
            if (eventExamDto == null)
                throw new ArgumentNullException(nameof(eventExamDto));

            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);

            if (parentEvent == null)
                throw new UnauthorizedAccessException("You are not authorized to access this event or it does not exist.");

            var existing = await _eventExamRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (existing != null)
                throw new InvalidOperationException($"An exam record already exists for the event with ID {eventId}.");

            var entity = _mapper.Map<EventExam>(eventExamDto);
            entity.EventId = eventId;

            var created = await _eventExamRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<EventExamDto>(created);
        }

        public async Task<EventExamDto> UpdateAsync(Guid eventId, UpdateEventExamDto eventExamDto, CancellationToken cancellationToken)
        {
            if (eventExamDto == null)
                throw new ArgumentNullException(nameof(eventExamDto));

            var exam = await _eventExamRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException($"No exam record found for the event with ID {eventId}.");

            exam.Type = eventExamDto.Type;

            var updated = await _eventExamRepository.UpdateAsync(exam, cancellationToken);

            return _mapper.Map<EventExamDto>(updated);
        }

        public async Task DeleteAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var exam = await _eventExamRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (exam == null)
                throw new KeyNotFoundException($"No exam record found for the event with ID {eventId}.");

            await _eventExamRepository.DeleteAsync(exam.Id, cancellationToken);
        }
    }
}
