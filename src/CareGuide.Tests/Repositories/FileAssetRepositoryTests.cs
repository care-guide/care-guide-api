using CareGuide.Infra.Contexts;
using CareGuide.Infra.Repositories;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Tests.Repositories;

public class FileAssetRepositoryTests
{
    private readonly Guid _ownerId = Guid.NewGuid();
    private readonly Guid _otherPersonId = Guid.NewGuid();
    private readonly string _databaseName = Guid.NewGuid().ToString();

    [Fact]
    public async Task Operations_CannotReadUpdateOrDeleteAnotherPersonsFile()
    {
        var foreignFile = CreateFile(_otherPersonId);
        await using (var seedContext = CreateContext())
        {
            seedContext.FileAssets.Add(foreignFile);
            await seedContext.SaveChangesAsync();
        }

        await using (var context = CreateContext())
        {
            var repository = CreateRepository(context, _ownerId);

            (await repository.GetAsync(foreignFile.Id)).Should().BeNull();
            (await repository.DeleteAsync(foreignFile.Id)).Should().BeNull();
        }

        await using (var verificationContext = CreateContext())
        {
            var adminRepository = CreateRepository(verificationContext, _ownerId, true);
            var persisted = await adminRepository.GetAsync(foreignFile.Id);
            persisted.Should().NotBeNull();

            persisted!.Name = "changed.pdf";
            await using var ownerContext = CreateContext(_ownerId);
            var ownerRepository = CreateRepository(ownerContext, _ownerId);
            var act = async () => await ownerRepository.UpdateAsync(persisted);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }
    }

    private DatabaseContext CreateContext(Guid? sessionPersonId = null)
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(_databaseName).Options;
        if (!sessionPersonId.HasValue) return new DatabaseContext(options);

        var session = Substitute.For<IUserSessionContext>();
        session.PersonId.Returns(sessionPersonId.Value);
        return new DatabaseContext(options, session);
    }

    private static FileAssetRepository CreateRepository(DatabaseContext context, Guid personId, bool isAdmin = false)
    {
        var session = Substitute.For<IUserSessionContext>();
        session.PersonId.Returns(personId);
        session.IsAdmin.Returns(isAdmin);
        return new FileAssetRepository(context, new PersonQueryScope(session));
    }

    private static FileAsset CreateFile(Guid personId) => new()
    {
        PersonId = personId,
        Name = "report.pdf",
        Size = 42,
        Hash = new string('a', 64),
        StorageKey = $"files/{Guid.NewGuid():N}",
        ContentType = "application/pdf",
        StorageProvider = StorageProvider.CloudflareR2,
        UploadStatus = FileUploadStatus.Uploaded
    };
}
