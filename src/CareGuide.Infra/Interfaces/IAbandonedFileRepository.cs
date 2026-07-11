using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces;

public interface IAbandonedFileRepository
{
    Task<List<FileAsset>> GetExpiredPendingAsync(DateTime createdBefore, CancellationToken cancellationToken);
    Task<List<FileAsset>> GetExpiredUnreferencedAsync(DateTime createdBefore, CancellationToken cancellationToken);
    Task DeleteAsync(FileAsset file, CancellationToken cancellationToken);
}
