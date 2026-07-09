using CareGuide.Models.DTOs.Event;

namespace CareGuide.Core.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<EventDto> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<EventDto> CreateAsync(CreateEventDto eventDto, CancellationToken cancellationToken);
        Task<EventDto> UpdateAsync(Guid id, UpdateEventDto eventDto, CancellationToken cancellationToken);
        Task DeleteAllByPersonAsync(CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}
