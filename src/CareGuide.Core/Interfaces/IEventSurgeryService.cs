using CareGuide.Models.DTOs.EventSurgery;

namespace CareGuide.Core.Interfaces
{
    public interface IEventSurgeryService
    {
        Task<EventSurgeryDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        Task<EventSurgeryDto> CreateAsync(Guid eventId, CreateEventSurgeryDto eventSurgeryDto, CancellationToken cancellationToken);
        Task<EventSurgeryDto> UpdateAsync(Guid eventId, UpdateEventSurgeryDto eventSurgeryDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
