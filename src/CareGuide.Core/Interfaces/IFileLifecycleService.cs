namespace CareGuide.Core.Interfaces;

public interface IFileLifecycleService
{
    Task DeleteAsync(Guid fileId, CancellationToken cancellationToken);
}
