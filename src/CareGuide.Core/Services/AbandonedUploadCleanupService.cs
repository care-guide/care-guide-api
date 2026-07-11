using CareGuide.Core.Interfaces;
using CareGuide.Core.Interfaces.Storage;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Configuration;
using Microsoft.Extensions.Options;

namespace CareGuide.Core.Services;

public sealed class AbandonedUploadCleanupService(
    IAbandonedFileRepository repository,
    IObjectStorage objectStorage,
    IOptions<FileLifecycleOptions> options) : IAbandonedUploadCleanupService
{
    private readonly FileLifecycleOptions _options = options.Value;

    public async Task<int> CleanupAsync(CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-_options.PendingUploadExpirationMinutes);
        var retentionCutoff = DateTime.UtcNow.AddDays(-_options.UnreferencedFileRetentionDays);
        var pending = await repository.GetExpiredPendingAsync(cutoff, cancellationToken);
        var unreferenced = await repository.GetExpiredUnreferencedAsync(retentionCutoff, cancellationToken);
        var candidates = pending.Concat(unreferenced).DistinctBy(file => file.Id).ToList();
        var deleted = 0;

        foreach (var file in candidates)
        {
            await objectStorage.DeleteAsync(file.StorageKey, cancellationToken);
            await repository.DeleteAsync(file, cancellationToken);
            deleted++;
        }

        return deleted;
    }
}
