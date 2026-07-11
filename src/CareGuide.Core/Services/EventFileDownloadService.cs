using CareGuide.Core.Interfaces;
using CareGuide.Core.Interfaces.Storage;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Configuration;
using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.Enums;
using Microsoft.Extensions.Options;

namespace CareGuide.Core.Services;

public sealed class EventFileDownloadService(
    IEventFileRepository eventFileRepository,
    IObjectStorage objectStorage,
    IOptions<FileDownloadOptions> options) : IEventFileDownloadService
{
    private readonly FileDownloadOptions _options = options.Value;

    public async Task<EventFileDownloadDto> CreateDownloadAsync(
        Guid eventId,
        Guid fileId,
        CancellationToken cancellationToken)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("The event id cannot be empty.", nameof(eventId));
        if (fileId == Guid.Empty) throw new ArgumentException("The file id cannot be empty.", nameof(fileId));

        var attachment = await eventFileRepository.GetByEventAndFileAsync(eventId, fileId, cancellationToken);
        if (attachment?.File == null)
            throw new KeyNotFoundException("The requested event file was not found.");
        if (attachment.File.UploadStatus != FileUploadStatus.Uploaded)
            throw new InvalidOperationException("The requested file is not available for download.");

        var metadata = await objectStorage.GetMetadataAsync(attachment.File.StorageKey, cancellationToken);
        if (metadata == null)
            throw new KeyNotFoundException("The requested object is not available in private storage.");

        var expiresIn = TimeSpan.FromSeconds(_options.PresignedUrlExpirationSeconds);
        var expiresAt = DateTimeOffset.UtcNow.Add(expiresIn);
        var url = objectStorage.CreateDownloadUrl(attachment.File.StorageKey, attachment.File.Name, expiresIn);

        return new EventFileDownloadDto(fileId, url, expiresAt);
    }
}
