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
    }
}
