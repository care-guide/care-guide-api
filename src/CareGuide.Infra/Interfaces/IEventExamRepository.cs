using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventExamRepository : IRepository<EventExam>
    {
        Task<EventExam?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
