using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.Repositories.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Infra.Repositories
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        private readonly DatabaseContext _context;

        public AddressRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }
    }
}
