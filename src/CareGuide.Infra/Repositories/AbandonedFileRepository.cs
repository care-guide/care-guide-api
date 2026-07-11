using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories;

public sealed class AbandonedFileRepository(DatabaseContext context) : IAbandonedFileRepository
{
    public Task<List<FileAsset>> GetExpiredPendingAsync(DateTime createdBefore, CancellationToken cancellationToken) =>
        context.FileAssets
            .Where(file =>
                file.UploadStatus == FileUploadStatus.Pending &&
                file.CreatedAt < createdBefore &&
                !file.EventFiles.Any())
            .ToListAsync(cancellationToken);

    public Task<List<FileAsset>> GetExpiredUnreferencedAsync(DateTime createdBefore, CancellationToken cancellationToken) =>
        context.FileAssets
            .Where(file =>
                file.UploadStatus == FileUploadStatus.Uploaded &&
                file.UpdatedAt < createdBefore &&
                !file.EventFiles.Any())
            .ToListAsync(cancellationToken);

    public async Task DeleteAsync(FileAsset file, CancellationToken cancellationToken)
    {
        context.FileAssets.Remove(file);
        await context.SaveChangesAsync(cancellationToken);
    }
}
