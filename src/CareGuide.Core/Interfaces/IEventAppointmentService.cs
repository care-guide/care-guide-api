using CareGuide.Models.DTOs.EventAppointment;

namespace CareGuide.Core.Interfaces
{
    public interface IEventAppointmentService
    {
        Task<EventAppointmentDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        Task<EventAppointmentDto> CreateAsync(Guid eventId, CreateEventAppointmentDto eventAppointmentDto, CancellationToken cancellationToken);
        Task<EventAppointmentDto> UpdateAsync(Guid eventId, UpdateEventAppointmentDto eventAppointmentDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
