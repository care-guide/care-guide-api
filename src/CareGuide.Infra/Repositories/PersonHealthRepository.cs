using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Repositories
{
    public class PersonHealthRepository : BasePersonOwnedRepository<PersonHealth>, IPersonHealthRepository
    {
        private readonly DatabaseContext _context;

        public PersonHealthRepository(DatabaseContext context, IQueryScope queryScope) : base(context, queryScope)
        {
            _context = context;
        }
    }
}
