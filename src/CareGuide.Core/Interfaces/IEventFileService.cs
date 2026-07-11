using CareGuide.Models.DTOs.EventFile;

namespace CareGuide.Core.Interfaces
{
    public interface IEventFileService
    {
        Task<IReadOnlyCollection<EventFileDto>> GetAllByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        Task<EventFileDto> AttachAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken);
        Task<EventFileDto> ConfirmUploadAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken);
        Task DetachAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken);
    }
}
