using CareGuide.Models.DTOs.EventMedicine;

namespace CareGuide.Core.Interfaces
{
    public interface IEventMedicineService
    {
        Task<EventMedicineDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        Task<EventMedicineDto> CreateAsync(Guid eventId, CreateEventMedicineDto eventMedicineDto, CancellationToken cancellationToken);
        Task<EventMedicineDto> UpdateAsync(Guid eventId, UpdateEventMedicineDto eventMedicineDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
