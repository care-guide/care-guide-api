using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventAddressRepository
    {
        Task<EventAddress> AddAsync(EventAddress entity, CancellationToken cancellationToken = default);
        Task<List<EventAddress>> GetAllByEventAsync(int page, int pageSize, Guid eventId, CancellationToken cancellationToken = default);
        Task<int> CountByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task<EventAddress?> GetByEventAndAddressAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken = default);
        Task DeleteAsync(EventAddress entity, CancellationToken cancellationToken = default);
    }
}
