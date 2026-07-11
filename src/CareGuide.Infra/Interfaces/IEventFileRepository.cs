using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventFileRepository
    {
        Task<EventFile> AddAsync(EventFile entity, CancellationToken cancellationToken = default);
        Task<List<EventFile>> GetAllByEventAsync(Guid eventId, CancellationToken cancellationToken = default);
        Task<EventFile?> GetByEventAndFileAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken = default);
        Task DeleteAsync(EventFile entity, CancellationToken cancellationToken = default);
        Task<int> CountByFileAsync(Guid fileId, CancellationToken cancellationToken = default);
    }
}
