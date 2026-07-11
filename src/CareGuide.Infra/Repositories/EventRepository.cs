using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class EventRepository : BasePersonOwnedRepository<Event>, IEventRepository
    {
        public EventRepository(DatabaseContext context, IQueryScope queryScope) : base(context, queryScope)
        {
        }

        public async Task<List<Event>> GetAllByPersonAsync(
            DateTime? from,
            DateTime? to,
            EventSortOrder sortOrder,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);

            var query = Scoped;

            if (from.HasValue)
                query = query.Where(e => e.StartDate >= from.Value);

            if (to.HasValue)
                query = query.Where(e => e.StartDate <= to.Value);

            query = sortOrder == EventSortOrder.Desc
                ? query.OrderByDescending(e => e.StartDate)
                : query.OrderBy(e => e.StartDate);

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }
    }
}
