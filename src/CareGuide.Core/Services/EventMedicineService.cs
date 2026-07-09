using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventMedicine;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class EventMedicineService : IEventMedicineService
    {
        private readonly IEventMedicineRepository _eventMedicineRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventMedicineService(IEventMedicineRepository eventMedicineRepository, IEventRepository eventRepository, IMapper mapper)
        {
            _eventMedicineRepository = eventMedicineRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventMedicineDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var medicine = await _eventMedicineRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (medicine == null)
                throw new KeyNotFoundException($"No medicine record found for the event with ID {eventId}.");

            return _mapper.Map<EventMedicineDto>(medicine);
        }

        public async Task<EventMedicineDto> CreateAsync(Guid eventId, CreateEventMedicineDto eventMedicineDto, CancellationToken cancellationToken)
        {
            if (eventMedicineDto == null)
                throw new ArgumentNullException(nameof(eventMedicineDto));

            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);

            if (parentEvent == null)
                throw new UnauthorizedAccessException("You are not authorized to access this event or it does not exist.");

            var existing = await _eventMedicineRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (existing != null)
                throw new InvalidOperationException($"A medicine record already exists for the event with ID {eventId}.");

            var entity = _mapper.Map<EventMedicine>(eventMedicineDto);
            entity.EventId = eventId;

            var created = await _eventMedicineRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<EventMedicineDto>(created);
        }

        public async Task<EventMedicineDto> UpdateAsync(Guid eventId, UpdateEventMedicineDto eventMedicineDto, CancellationToken cancellationToken)
        {
            if (eventMedicineDto == null)
                throw new ArgumentNullException(nameof(eventMedicineDto));

            var medicine = await _eventMedicineRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (medicine == null)
                throw new KeyNotFoundException($"No medicine record found for the event with ID {eventId}.");

            medicine.Dosage = eventMedicineDto.Dosage;
            medicine.Laboratory = eventMedicineDto.Laboratory;

            var updated = await _eventMedicineRepository.UpdateAsync(medicine, cancellationToken);

            return _mapper.Map<EventMedicineDto>(updated);
        }

        public async Task DeleteAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var medicine = await _eventMedicineRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (medicine == null)
                throw new KeyNotFoundException($"No medicine record found for the event with ID {eventId}.");

            await _eventMedicineRepository.DeleteAsync(medicine.Id, cancellationToken);
        }
    }
}
