using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IEventRepository : IBasePersonOwnedRepository<Event>
    {
    }
}
