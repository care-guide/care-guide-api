using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Core.Interfaces
{
    public interface IEventFileService
    {
        Task<PagedResult<EventFileDto>> GetAllByEventIdAsync(int page, int pageSize, Guid eventId, CancellationToken cancellationToken);
        Task<EventFileDto> AttachAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken);
        Task<EventFileDto> ConfirmUploadAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken);
        Task DetachAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken);
    }
}
