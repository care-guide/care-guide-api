using CareGuide.Models.DTOs.EventExam;

namespace CareGuide.Core.Interfaces
{
    public interface IEventExamService
    {
        Task<EventExamDto> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        Task<EventExamDto> CreateAsync(Guid eventId, CreateEventExamDto eventExamDto, CancellationToken cancellationToken);
        Task<EventExamDto> UpdateAsync(Guid eventId, UpdateEventExamDto eventExamDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
