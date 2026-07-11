using CareGuide.Models.DTOs.FileAsset;

namespace CareGuide.Core.Interfaces;

public interface IEventFileUploadService
{
    Task<InitiateEventFileUploadResponseDto> InitiateAsync(
        Guid eventId,
        InitiateEventFileUploadDto dto,
        CancellationToken cancellationToken);
}
