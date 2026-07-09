using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.EventAppointment;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class EventAppointmentService : IEventAppointmentService
    {
        private readonly IEventAppointmentRepository _eventAppointmentRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventAppointmentService(IEventAppointmentRepository eventAppointmentRepository, IEventRepository eventRepository, IMapper mapper)
        {
            _eventAppointmentRepository = eventAppointmentRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventAppointmentDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var appointment = await _eventAppointmentRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (appointment == null)
                throw new KeyNotFoundException($"No appointment found for the event with ID {eventId}.");

            return _mapper.Map<EventAppointmentDto>(appointment);
        }

        public async Task<EventAppointmentDto> CreateAsync(Guid eventId, CreateEventAppointmentDto eventAppointmentDto, CancellationToken cancellationToken)
        {
            if (eventAppointmentDto == null)
                throw new ArgumentNullException(nameof(eventAppointmentDto));

            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);

            if (parentEvent == null)
                throw new UnauthorizedAccessException("You are not authorized to access this event or it does not exist.");

            var existing = await _eventAppointmentRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (existing != null)
                throw new InvalidOperationException($"An appointment already exists for the event with ID {eventId}.");

            var entity = _mapper.Map<EventAppointment>(eventAppointmentDto);
            entity.EventId = eventId;

            var created = await _eventAppointmentRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<EventAppointmentDto>(created);
        }

        public async Task<EventAppointmentDto> UpdateAsync(Guid eventId, UpdateEventAppointmentDto eventAppointmentDto, CancellationToken cancellationToken)
        {
            if (eventAppointmentDto == null)
                throw new ArgumentNullException(nameof(eventAppointmentDto));

            var appointment = await _eventAppointmentRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (appointment == null)
                throw new KeyNotFoundException($"No appointment found for the event with ID {eventId}.");

            appointment.Doctor = eventAppointmentDto.Doctor;
            appointment.Type = eventAppointmentDto.Type;

            var updated = await _eventAppointmentRepository.UpdateAsync(appointment, cancellationToken);

            return _mapper.Map<EventAppointmentDto>(updated);
        }

        public async Task DeleteAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var appointment = await _eventAppointmentRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (appointment == null)
                throw new KeyNotFoundException($"No appointment found for the event with ID {eventId}.");

            await _eventAppointmentRepository.DeleteAsync(appointment.Id, cancellationToken);
        }
    }
}
