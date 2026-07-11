using CareGuide.Core.Interfaces;
using CareGuide.Core.Interfaces.Storage;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Configuration;
using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Security.Interfaces;
using Microsoft.Extensions.Options;

namespace CareGuide.Core.Services;

public sealed class EventFileUploadService(
    IEventRepository eventRepository,
    IFileAssetRepository fileAssetRepository,
    IObjectStorage objectStorage,
    IUserSessionContext userSession,
    IOptions<FileUploadOptions> options) : IEventFileUploadService
{
    private readonly FileUploadOptions _options = options.Value;

    public async Task<InitiateEventFileUploadResponseDto> InitiateAsync(
        Guid eventId,
        InitiateEventFileUploadDto dto,
        CancellationToken cancellationToken)
    {
        if (eventId == Guid.Empty) throw new ArgumentException("The event id cannot be empty.", nameof(eventId));
        ArgumentNullException.ThrowIfNull(dto);

        var eventEntity = await eventRepository.GetAsync(eventId, cancellationToken);
        if (eventEntity is null)
            throw new KeyNotFoundException($"No event found with the ID {eventId}.");

        var fileId = Guid.NewGuid();
        var storageKey = $"person/{userSession.PersonId:N}/event/{eventId:N}/{fileId:N}";
        var file = new FileAsset
        {
            Id = fileId,
            PersonId = userSession.PersonId,
            Name = dto.Name,
            Size = dto.Size,
            Hash = dto.ExpectedHash.ToLowerInvariant(),
            ContentType = dto.ContentType,
            StorageKey = storageKey,
            StorageProvider = StorageProvider.CloudflareR2,
            UploadStatus = FileUploadStatus.Pending
        };

        await fileAssetRepository.AddAsync(file, cancellationToken);

        var expiresIn = TimeSpan.FromSeconds(_options.PresignedUrlExpirationSeconds);
        var expiresAt = DateTimeOffset.UtcNow.Add(expiresIn);
        var uploadUrl = objectStorage.CreateUploadUrl(storageKey, dto.ContentType, expiresIn);

        return new InitiateEventFileUploadResponseDto(
            file.Id,
            uploadUrl,
            "PUT",
            new Dictionary<string, string> { ["Content-Type"] = dto.ContentType },
            expiresAt);
    }
}
