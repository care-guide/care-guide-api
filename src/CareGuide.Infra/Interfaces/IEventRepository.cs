using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventRepository : IBasePersonOwnedRepository<Event>
    {
        Task<List<Event>> GetAllByPersonAsync(
            DateTime? from,
            DateTime? to,
            EventSortOrder sortOrder,
            int page,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
