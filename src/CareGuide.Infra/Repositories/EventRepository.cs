using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Repositories
{
    public class EventRepository : BasePersonOwnedRepository<Event>, IEventRepository
    {
        public EventRepository(DatabaseContext context, IQueryScope queryScope) : base(context, queryScope)
        {
        }
    }
}
