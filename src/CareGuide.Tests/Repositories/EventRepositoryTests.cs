using CareGuide.Infra.Contexts;
using CareGuide.Infra.Repositories;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Tests.Repositories;

public class EventRepositoryTests
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

    private static EventRepository CreateRepository(DatabaseContext context, Guid personId, bool isAdmin = false)
    {
        var session = Substitute.For<IUserSessionContext>();
        session.PersonId.Returns(personId);
        session.IsAdmin.Returns(isAdmin);

        return new EventRepository(context, new PersonQueryScope(session));
    }

    private static Event CreateEvent(Guid personId, DateTime startDate, string name) => new()
    {
        PersonId = personId,
        Name = name,
        StartDate = startDate,
        Type = EventType.Appointment
    };

    private async Task SeedAsync(params Event[] events)
    {
        using var context = CreateContext();
        context.Events.AddRange(events);
        await context.SaveChangesAsync();
    }

    [Fact(DisplayName = "GetAllByPersonAsync: filters events within the inclusive from/to window")]
    public async Task GetAllByPersonAsync_WithDateRange_ReturnsOnlyEventsInWindow()
    {
        var before = CreateEvent(_personAId, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), "Before");
        var lowerBound = CreateEvent(_personAId, new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc), "LowerBound");
        var inside = CreateEvent(_personAId, new DateTime(2026, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Inside");
        var upperBound = CreateEvent(_personAId, new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc), "UpperBound");
        var after = CreateEvent(_personAId, new DateTime(2026, 1, 20, 0, 0, 0, DateTimeKind.Utc), "After");

        await SeedAsync(before, lowerBound, inside, upperBound, after);

        using var context = CreateContext();
        var repository = CreateRepository(context, _personAId);

        var result = await repository.GetAllByPersonAsync(
            from: new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
            to: new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc),
            sortOrder: EventSortOrder.Asc,
            page: 1,
            pageSize: 10,
            cancellationToken: CancellationToken.None);

        result.Select(e => e.Name).Should().BeEquivalentTo(["LowerBound", "Inside", "UpperBound"]);
    }

    [Fact(DisplayName = "GetAllByPersonAsync: orders ascending by startDate by default")]
    public async Task GetAllByPersonAsync_Ascending_OrdersByStartDateAscending()
    {
        var first = CreateEvent(_personAId, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), "First");
        var second = CreateEvent(_personAId, new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), "Second");
        var third = CreateEvent(_personAId, new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc), "Third");

        await SeedAsync(third, first, second);

        using var context = CreateContext();
        var repository = CreateRepository(context, _personAId);

        var result = await repository.GetAllByPersonAsync(null, null, EventSortOrder.Asc, 1, 10, CancellationToken.None);

        result.Select(e => e.Name).Should().ContainInOrder("First", "Second", "Third");
    }

    [Fact(DisplayName = "GetAllByPersonAsync: orders descending by startDate when requested")]
    public async Task GetAllByPersonAsync_Descending_OrdersByStartDateDescending()
    {
        var first = CreateEvent(_personAId, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), "First");
        var second = CreateEvent(_personAId, new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc), "Second");
        var third = CreateEvent(_personAId, new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc), "Third");

        await SeedAsync(first, second, third);

        using var context = CreateContext();
        var repository = CreateRepository(context, _personAId);

        var result = await repository.GetAllByPersonAsync(null, null, EventSortOrder.Desc, 1, 10, CancellationToken.None);

        result.Select(e => e.Name).Should().ContainInOrder("Third", "Second", "First");
    }

    [Fact(DisplayName = "GetAllByPersonAsync: composes date filter with pagination")]
    public async Task GetAllByPersonAsync_WithDateRangeAndPagination_ReturnsPagedWindow()
    {
        var events = Enumerable.Range(0, 5)
            .Select(i => CreateEvent(_personAId, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(i), $"Event{i}"))
            .ToArray();

        await SeedAsync(events);

        using var context = CreateContext();
        var repository = CreateRepository(context, _personAId);

        var result = await repository.GetAllByPersonAsync(
            from: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            to: new DateTime(2026, 1, 5, 0, 0, 0, DateTimeKind.Utc),
            sortOrder: EventSortOrder.Asc,
            page: 2,
            pageSize: 2,
            cancellationToken: CancellationToken.None);

        result.Select(e => e.Name).Should().ContainInOrder("Event2", "Event3");
    }

    [Fact(DisplayName = "GetAllByPersonAsync: never returns events belonging to another person")]
    public async Task GetAllByPersonAsync_OnlyReturnsEventsOwnedBySessionPerson()
    {
        var ownEvent = CreateEvent(_personAId, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), "Own");
        var otherEvent = CreateEvent(_personBId, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), "Other");

        await SeedAsync(ownEvent, otherEvent);

        using var context = CreateContext();
        var repository = CreateRepository(context, _personAId);

        var result = await repository.GetAllByPersonAsync(null, null, EventSortOrder.Asc, 1, 10, CancellationToken.None);

        result.Should().ContainSingle(e => e.Name == "Own");
    }
}
