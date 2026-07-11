namespace CareGuide.Core.Interfaces;

public interface IAbandonedUploadCleanupService
{
    Task<int> CleanupAsync(CancellationToken cancellationToken);
}
