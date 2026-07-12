namespace CareGuide.Models.DTOs.Shared
{
    public record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int Total)
    {
        public bool HasNext => (long)Page * PageSize < Total;
    }
}
