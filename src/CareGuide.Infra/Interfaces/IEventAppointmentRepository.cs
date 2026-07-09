using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventAppointmentRepository : IRepository<EventAppointment>
    {
        Task<EventAppointment?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
