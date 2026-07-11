using CareGuide.Core.Interfaces.Storage;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Configuration;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.Extensions.Options;

namespace CareGuide.Tests.Services;

public class AbandonedUploadCleanupServiceTests
{
    [Fact]
    public async Task CleanupAsync_WhenRepeated_ShouldDeleteOnlyReturnedCandidates()
    {
        var repository = Substitute.For<IAbandonedFileRepository>();
        var storage = Substitute.For<IObjectStorage>();
        var pending = CreateFile(FileUploadStatus.Pending);
        repository.GetExpiredPendingAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(_ => [pending], _ => []);
        repository.GetExpiredUnreferencedAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(_ => [], _ => []);
        var service = CreateService(repository, storage);

        var first = await service.CleanupAsync(CancellationToken.None);
        var second = await service.CleanupAsync(CancellationToken.None);

        first.Should().Be(1);
        second.Should().Be(0);
        await storage.Received(1).DeleteAsync(pending.StorageKey, Arg.Any<CancellationToken>());
        await repository.Received(1).DeleteAsync(pending, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CleanupAsync_WhenStorageFails_ShouldNotDeleteDatabaseRecord()
    {
        var repository = Substitute.For<IAbandonedFileRepository>();
        var storage = Substitute.For<IObjectStorage>();
        var pending = CreateFile(FileUploadStatus.Pending);
        repository.GetExpiredPendingAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns([pending]);
        repository.GetExpiredUnreferencedAsync(Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns([]);
        storage.DeleteAsync(pending.StorageKey, Arg.Any<CancellationToken>()).ThrowsAsync(new HttpRequestException());
        var service = CreateService(repository, storage);

        var act = () => service.CleanupAsync(CancellationToken.None);

        await act.Should().ThrowAsync<HttpRequestException>();
        await repository.DidNotReceive().DeleteAsync(Arg.Any<FileAsset>(), Arg.Any<CancellationToken>());
    }

    private static AbandonedUploadCleanupService CreateService(IAbandonedFileRepository repository, IObjectStorage storage) =>
        new(repository, storage, Options.Create(new FileLifecycleOptions
        {
            PendingUploadExpirationMinutes = 60,
            UnreferencedFileRetentionDays = 30,
            CleanupIntervalMinutes = 15
        }));

    private static FileAsset CreateFile(FileUploadStatus status) => new()
    {
        Id = Guid.NewGuid(),
        PersonId = Guid.NewGuid(),
        Name = "exam.pdf",
        Size = 1,
        Hash = new string('a', 64),
        StorageKey = $"files/{Guid.NewGuid():N}",
        ContentType = "application/pdf",
        UploadStatus = status
    };
}
