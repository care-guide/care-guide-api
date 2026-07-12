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
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public EventAppointmentService(IEventAppointmentRepository eventAppointmentRepository, IEventRepository eventRepository, IDoctorRepository doctorRepository, IMapper mapper)
        {
            _eventAppointmentRepository = eventAppointmentRepository;
            _eventRepository = eventRepository;
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        private async Task EnsureDoctorOwnedAsync(Guid? doctorId, CancellationToken cancellationToken)
        {
            if (!doctorId.HasValue)
                return;

            var doctor = await _doctorRepository.GetAsync(doctorId.Value, cancellationToken);

            if (doctor == null)
                throw new UnauthorizedAccessException("You are not authorized to link this doctor or it does not exist.");
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

            await EnsureDoctorOwnedAsync(eventAppointmentDto.DoctorId, cancellationToken);

            var entity = _mapper.Map<EventAppointment>(eventAppointmentDto);
            entity.EventId = eventId;

            await _eventAppointmentRepository.AddAsync(entity, cancellationToken);

            var created = await _eventAppointmentRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (created == null)
                throw new InvalidOperationException("The created appointment could not be retrieved.");

            return _mapper.Map<EventAppointmentDto>(created);
        }

        public async Task<EventAppointmentDto> UpdateAsync(Guid eventId, UpdateEventAppointmentDto eventAppointmentDto, CancellationToken cancellationToken)
        {
            if (eventAppointmentDto == null)
                throw new ArgumentNullException(nameof(eventAppointmentDto));

            var appointment = await _eventAppointmentRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (appointment == null)
                throw new KeyNotFoundException($"No appointment found for the event with ID {eventId}.");

            await EnsureDoctorOwnedAsync(eventAppointmentDto.DoctorId, cancellationToken);

            appointment.Doctor = eventAppointmentDto.Doctor;
            appointment.DoctorId = eventAppointmentDto.DoctorId;
            appointment.Type = eventAppointmentDto.Type;

            await _eventAppointmentRepository.UpdateAsync(appointment, cancellationToken);

            var updated = await _eventAppointmentRepository.GetByEventIdAsync(eventId, cancellationToken);

            if (updated == null)
                throw new InvalidOperationException("The updated appointment could not be retrieved.");

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
