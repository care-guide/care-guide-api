using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventMedicineRepository : IRepository<EventMedicine>
    {
        Task<EventMedicine?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
