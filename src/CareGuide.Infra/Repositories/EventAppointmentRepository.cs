using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class EventAppointmentRepository : BaseRepository<EventAppointment>, IEventAppointmentRepository
    {
        private readonly IUserSessionContext _userSessionContext;

        public EventAppointmentRepository(DatabaseContext context, IUserSessionContext userSessionContext) : base(context)
        {
            _userSessionContext = userSessionContext;
        }

        public async Task<EventAppointment?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await context.Set<EventAppointment>()
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.EventId == eventId && x.Event.PersonId == personId, cancellationToken);
        }
    }
}
