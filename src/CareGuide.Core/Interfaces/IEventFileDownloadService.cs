using CareGuide.Models.DTOs.EventFile;

namespace CareGuide.Core.Interfaces;

public interface IEventFileDownloadService
{
    Task<EventFileDownloadDto> CreateDownloadAsync(Guid eventId, Guid fileId, CancellationToken cancellationToken);
}
