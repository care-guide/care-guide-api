using CareGuide.Core.Interfaces;
using CareGuide.Core.Interfaces.Storage;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Enums;

namespace CareGuide.Core.Services;

public sealed class FileLifecycleService(
    IFileAssetRepository fileRepository,
    IEventFileRepository eventFileRepository,
    IObjectStorage objectStorage) : IFileLifecycleService
{
    public async Task DeleteAsync(Guid fileId, CancellationToken cancellationToken)
    {
        if (fileId == Guid.Empty) throw new ArgumentException("The file id cannot be empty.", nameof(fileId));

        var file = await fileRepository.GetAsync(fileId, cancellationToken);
        if (file == null) throw new KeyNotFoundException("The requested file was not found.");
        if (await eventFileRepository.CountByFileAsync(fileId, cancellationToken) > 0)
            throw new InvalidOperationException("The file cannot be deleted while it is attached to an event.");

        file.UploadStatus = FileUploadStatus.DeletionPending;
        await fileRepository.UpdateAsync(file, cancellationToken);

        try
        {
            await objectStorage.DeleteAsync(file.StorageKey, cancellationToken);
        }
        catch
        {
            file.UploadStatus = FileUploadStatus.DeletionFailed;
            await fileRepository.UpdateAsync(file, cancellationToken);
            throw;
        }

        await fileRepository.DeleteAsync(fileId, cancellationToken);
    }
}
