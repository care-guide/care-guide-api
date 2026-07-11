using CareGuide.Infra.Contexts;
using CareGuide.Infra.Repositories;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Tests.Repositories
{
    public class BasePersonOwnedRepositoryTests
    {
        private readonly Guid _personAId = Guid.NewGuid();
        private readonly Guid _personBId = Guid.NewGuid();
        private readonly string _databaseName = Guid.NewGuid().ToString();

        private DatabaseContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            return new DatabaseContext(options);
        }

        private PersonDiseaseRepository CreateRepository(DatabaseContext context, Guid personId, bool isAdmin = false)
        {
            var session = Substitute.For<IUserSessionContext>();
            session.PersonId.Returns(personId);
            session.IsAdmin.Returns(isAdmin);

            return new PersonDiseaseRepository(context, new PersonQueryScope(session));
        }

        private async Task<(PersonDisease diseaseA, PersonDisease diseaseB)> SeedAsync()
        {
            using var context = CreateContext();

            var diseaseA = new PersonDisease { PersonId = _personAId, Name = "Diabetes", DiseaseType = DiseaseType.Chronic };
            var diseaseB = new PersonDisease { PersonId = _personBId, Name = "Asthma", DiseaseType = DiseaseType.Respiratory };

            context.PersonDiseases.AddRange(diseaseA, diseaseB);
            await context.SaveChangesAsync();

            return (diseaseA, diseaseB);
        }

        [Fact]
        public async Task GetAsync_WhenEntityBelongsToAnotherPerson_ReturnsNull()
        {
            var (_, diseaseB) = await SeedAsync();

            using var context = CreateContext();
            var repository = CreateRepository(context, _personAId);

            var result = await repository.GetAsync(diseaseB.Id, CancellationToken.None);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAsync_WhenEntityBelongsToSessionPerson_ReturnsEntity()
        {
            var (diseaseA, _) = await SeedAsync();

            using var context = CreateContext();
            var repository = CreateRepository(context, _personAId);

            var result = await repository.GetAsync(diseaseA.Id, CancellationToken.None);

            result.Should().NotBeNull();
            result!.Id.Should().Be(diseaseA.Id);
        }

        [Fact]
        public async Task GetAsync_WhenSessionIsAdmin_ReturnsEntityFromAnyPerson()
        {
            var (_, diseaseB) = await SeedAsync();

            using var context = CreateContext();
            var repository = CreateRepository(context, _personAId, isAdmin: true);

            var result = await repository.GetAsync(diseaseB.Id, CancellationToken.None);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetManyAsync_OnlyReturnsEntitiesOwnedBySessionPerson()
        {
            var (diseaseA, diseaseB) = await SeedAsync();

            using var context = CreateContext();
            var repository = CreateRepository(context, _personAId);

            var result = await repository.GetManyAsync([diseaseA.Id, diseaseB.Id], 1, 10, CancellationToken.None);

            result.Should().ContainSingle(d => d.Id == diseaseA.Id);
        }

        [Fact]
        public async Task ExistsAsync_WhenEntityBelongsToAnotherPerson_ReturnsFalse()
        {
            var (_, diseaseB) = await SeedAsync();

            using var context = CreateContext();
            var repository = CreateRepository(context, _personAId);

            var result = await repository.ExistsAsync(diseaseB.Id, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteManyAsync_OnlyDeletesEntitiesOwnedBySessionPerson()
        {
            var (diseaseA, diseaseB) = await SeedAsync();

            using (var context = CreateContext())
            {
                var repository = CreateRepository(context, _personAId);
                await repository.DeleteManyAsync([diseaseA.Id, diseaseB.Id], CancellationToken.None);
            }

            using (var context = CreateContext())
            {
                var adminRepository = CreateRepository(context, _personAId, isAdmin: true);

                (await adminRepository.GetAsync(diseaseA.Id, CancellationToken.None)).Should().BeNull();
                (await adminRepository.GetAsync(diseaseB.Id, CancellationToken.None)).Should().NotBeNull();
            }
        }

        [Fact]
        public async Task DeleteAsync_WhenEntityBelongsToAnotherPerson_DoesNotDeleteIt()
        {
            var (_, diseaseB) = await SeedAsync();

            using (var context = CreateContext())
            {
                var repository = CreateRepository(context, _personAId);
                var result = await repository.DeleteAsync(diseaseB.Id, CancellationToken.None);
                result.Should().BeNull();
            }

            using (var context = CreateContext())
            {
                var adminRepository = CreateRepository(context, _personAId, isAdmin: true);
                (await adminRepository.GetAsync(diseaseB.Id, CancellationToken.None)).Should().NotBeNull();
            }
        }
    }
}
