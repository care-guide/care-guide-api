namespace CareGuide.Models.DTOs.Shared
{
    /// <summary>
    /// Standard pagination envelope returned by every collection endpoint.
    /// <paramref name="Total"/> must always come from a real COUNT query scoped with the
    /// same predicate as <paramref name="Items"/>, never from <c>Items.Count</c>.
    /// </summary>
    public record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int Total)
    {
        public bool HasNext => (long)Page * PageSize < Total;
    }
}
