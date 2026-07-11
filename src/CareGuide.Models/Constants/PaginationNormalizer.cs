namespace CareGuide.Models.Constants
{
    /// <summary>
    /// Single source of truth for page/pageSize normalization. Used by repositories (both the
    /// entity-generic <c>BaseRepository</c>/<c>BasePersonOwnedRepository</c> and join-table
    /// repositories such as DoctorAddress, DoctorPhone, DoctorSpecialty, PersonPhone) and by
    /// services that need the normalized values to build a <see cref="DTOs.Shared.PagedResult{T}"/>.
    /// Never throws: out-of-range values are clamped instead.
    /// </summary>
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
