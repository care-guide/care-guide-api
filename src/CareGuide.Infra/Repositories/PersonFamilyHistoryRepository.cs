using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Repositories
{
    public class PersonFamilyHistoryRepository : BasePersonOwnedRepository<PersonFamilyHistory>, IPersonFamilyHistoryRepository
    {
        private readonly DatabaseContext _context;

        public PersonFamilyHistoryRepository(DatabaseContext context, IQueryScope queryScope) : base(context, queryScope)
        {
            _context = context;
        }
    }
}