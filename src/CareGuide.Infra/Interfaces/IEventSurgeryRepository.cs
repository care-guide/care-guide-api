using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventSurgeryRepository : IRepository<EventSurgery>
    {
        Task<EventSurgery?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
