namespace CareGuide.Models.Constants
{
    public static class PaginationNormalizer
    {
        public static (int Page, int PageSize) Normalize(int page, int pageSize)
        {
            var normalizedPage = page < 1 ? PaginationConstants.DefaultPage : page;
            var normalizedPageSize = pageSize < 1 ? PaginationConstants.DefaultPageSize : Math.Min(pageSize, PaginationConstants.MaxPageSize);

            return (normalizedPage, normalizedPageSize);
        }
    }
}
