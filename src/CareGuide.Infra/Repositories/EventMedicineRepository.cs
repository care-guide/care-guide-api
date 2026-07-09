using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class EventMedicineRepository : BaseRepository<EventMedicine>, IEventMedicineRepository
    {
        private readonly IUserSessionContext _userSessionContext;

        public EventMedicineRepository(DatabaseContext context, IUserSessionContext userSessionContext) : base(context)
        {
            _userSessionContext = userSessionContext;
        }

        public async Task<EventMedicine?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await context.Set<EventMedicine>()
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.Event.PersonId == personId, cancellationToken);
        }
    }
}
