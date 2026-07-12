using CareGuide.Infra.Contexts;
using CareGuide.Infra.Repositories;
using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Tests.Repositories
{
    public class BaseRepositoryTests
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        private DatabaseContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            return new DatabaseContext(options);
        }

        private async Task SeedAsync(int count)
        {
            using var context = CreateContext();

            for (var i = 0; i < count; i++)
                context.Phones.Add(new Phone { Number = $"99999{i:0000}", AreaCode = "11", Type = PhoneType.CEL });

            await context.SaveChangesAsync();
        }

        [Fact(DisplayName = "GetAllAsync: pageSize above MaxPageSize is clamped")]
        public async Task GetAllAsync_PageSizeAboveMax_IsClamped()
        {
            await SeedAsync(PaginationConstants.MaxPageSize + 20);

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var result = await repository.GetAllAsync(1, PaginationConstants.MaxPageSize + 50, CancellationToken.None);

            result.Should().HaveCount(PaginationConstants.MaxPageSize);
        }

        [Theory(DisplayName = "GetAllAsync: non-positive page/pageSize are normalized instead of throwing")]
        [InlineData(-1, -1)]
        [InlineData(0, 0)]
        public async Task GetAllAsync_NonPositiveValues_AreNormalized(int page, int pageSize)
        {
            await SeedAsync(PaginationConstants.DefaultPageSize + 5);

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var act = async () => await repository.GetAllAsync(page, pageSize, CancellationToken.None);

            await act.Should().NotThrowAsync();
            var result = await repository.GetAllAsync(page, pageSize, CancellationToken.None);
            result.Should().HaveCount(PaginationConstants.DefaultPageSize);
        }

        [Fact(DisplayName = "GetAllPagedAsync: total reflects full seeded count, not the page's item count")]
        public async Task GetAllPagedAsync_TotalReflectsFullCount_NotPageItemCount()
        {
            const int seeded = 27;
            await SeedAsync(seeded);

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var (items, total, page, pageSize) = await repository.GetAllPagedAsync(1, 10, CancellationToken.None);

            items.Should().HaveCount(10);
            total.Should().Be(seeded);
            total.Should().NotBe(items.Count);
            page.Should().Be(1);
            pageSize.Should().Be(10);
        }

        [Fact(DisplayName = "GetAllPagedAsync: last page returns remainder and total still reflects full count")]
        public async Task GetAllPagedAsync_LastPage_ReturnsRemainder()
        {
            const int seeded = 25;
            await SeedAsync(seeded);

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var (items, total, page, pageSize) = await repository.GetAllPagedAsync(3, 10, CancellationToken.None);

            items.Should().HaveCount(5);
            total.Should().Be(seeded);
            page.Should().Be(3);
            pageSize.Should().Be(10);
        }

        [Fact(DisplayName = "GetAllPagedAsync: pageSize above MaxPageSize is clamped and reflected in the returned PageSize")]
        public async Task GetAllPagedAsync_PageSizeAboveMax_IsClamped()
        {
            await SeedAsync(PaginationConstants.MaxPageSize + 20);

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var (items, total, page, pageSize) = await repository.GetAllPagedAsync(1, PaginationConstants.MaxPageSize + 50, CancellationToken.None);

            items.Should().HaveCount(PaginationConstants.MaxPageSize);
            pageSize.Should().Be(PaginationConstants.MaxPageSize);
            total.Should().Be(PaginationConstants.MaxPageSize + 20);
        }

        [Fact(DisplayName = "GetAllPagedAsync: page beyond the last page returns an empty item list but the true total")]
        public async Task GetAllPagedAsync_PageBeyondLastPage_ReturnsEmptyItemsWithRealTotal()
        {
            const int seeded = 5;
            await SeedAsync(seeded);

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var (items, total, _, _) = await repository.GetAllPagedAsync(5, 10, CancellationToken.None);

            items.Should().BeEmpty();
            total.Should().Be(seeded);
        }

        [Fact(DisplayName = "DeleteAsync: physically removes entity")]
        public async Task DeleteAsync_PhysicallyRemovesEntity()
        {
            using var seedContext = CreateContext();
            var phone = new Phone { Number = "999999999", AreaCode = "11", Type = PhoneType.CEL };
            seedContext.Phones.Add(phone);
            await seedContext.SaveChangesAsync();

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var deleted = await repository.DeleteAsync(phone.Id, CancellationToken.None);

            deleted.Should().NotBeNull();
            context.Phones.Should().NotContain(x => x.Id == phone.Id);
        }

        [Fact(DisplayName = "DeleteManyAsync: physically removes matching entities")]
        public async Task DeleteManyAsync_PhysicallyRemovesMatchingEntities()
        {
            using var seedContext = CreateContext();
            var deletedPhoneA = new Phone { Number = "999999991", AreaCode = "11", Type = PhoneType.CEL };
            var deletedPhoneB = new Phone { Number = "999999992", AreaCode = "11", Type = PhoneType.CEL };
            var remainingPhone = new Phone { Number = "999999993", AreaCode = "11", Type = PhoneType.CEL };
            seedContext.Phones.AddRange(deletedPhoneA, deletedPhoneB, remainingPhone);
            await seedContext.SaveChangesAsync();

            using var context = CreateContext();
            var repository = new PhoneRepository(context);

            var deleted = await repository.DeleteManyAsync([deletedPhoneA.Id, deletedPhoneB.Id], CancellationToken.None);

            deleted.Should().HaveCount(2);
            context.Phones.Should().NotContain(x => x.Id == deletedPhoneA.Id || x.Id == deletedPhoneB.Id);
            context.Phones.Should().ContainSingle(x => x.Id == remainingPhone.Id);
        }
    }
}
